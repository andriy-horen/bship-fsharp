module Bship.GameHubOld

open System
open System.Collections.Concurrent
open System.Security.Claims
open System.Threading.Tasks
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Logging

let addOrUpdateWaitingClient
    (logger: ILogger)
    (waitingClients: ConcurrentDictionary<string, HubCallerContext>)
    (context: HubCallerContext)
    =
    let userId = context.UserIdentifier
    let isWaiting, existingContext = waitingClients.TryGetValue userId

    if isWaiting && existingContext.ConnectionId <> context.ConnectionId then
        logger.LogInformation(userId, existingContext.ConnectionId)

        existingContext.Abort()

    logger.LogInformation(
        "Adding user {UserId} to the waiting list; connectionId {ConnectionId}",
        userId,
        context.ConnectionId
    )

    waitingClients.AddOrUpdate(userId, context, (fun key existingValue -> context))
    |> ignore

let tryGetWaitingClient (logger: ILogger) (waitingClients: ConcurrentDictionary<string, HubCallerContext>) =
    let client = waitingClients |> Seq.tryHead

    client
    |> Option.bind (fun kv ->
        let removed, client = waitingClients.TryRemove kv.Key

        if removed then
            logger.LogInformation(
                "Found waiting user {UserId}, removed for pairing; connectionId {ConnectionId}",
                client.UserIdentifier,
                client.ConnectionId
            )

            Some client
        else
            logger.LogWarning(
                "Found waiting user {UserId}, failed to remove for pairing; connectionId {ConnectionId}",
                client.UserIdentifier,
                client.ConnectionId
            )

            None)

let tryGetCurrentClient (waitingClients: ConcurrentDictionary<string, HubCallerContext>) (current: HubCallerContext) =
    let userId = current.User.FindFirstValue(ClaimTypes.NameIdentifier)

    let isAlreadyWaiting = waitingClients.ContainsKey userId
    if isAlreadyWaiting then None else Some current


let tryPairClients
    (logger: ILogger)
    (hub: Hub)
    (groupId: string)
    (games: ConcurrentDictionary<string, Tuple<HubCallerContext, HubCallerContext>>)
    (current: HubCallerContext)
    (other: HubCallerContext)
    =
    let added = games.TryAdd(groupId.ToString(), (current, other))

    if added then
        logger.LogInformation(
            "Creating a group {GroupId} with two clients {User1}, {User2}",
            groupId,
            current.UserIdentifier,
            other.UserIdentifier
        )

        Some(
            Task
                .WhenAll(
                    [ hub.Groups.AddToGroupAsync(current.ConnectionId, groupId)
                      hub.Groups.AddToGroupAsync(other.ConnectionId, groupId) ]
                )
                .ContinueWith(fun _ -> hub.Clients.Groups(groupId).SendAsync("joined"))
                .Unwrap()
        )
    else
        logger.LogError("Attempted to create a group {GroupId} but failed", groupId)
        None

type DisconnectedUser = { DisconnectedAt: DateTimeOffset }

type UserConnection =
    | ConnectedUser of ConnectionContext: HubCallerContext
    | DisconnectedUser of DisconnectedUser

let addToWaiting (waitingUsers: ConcurrentDictionary<string, HubCallerContext>) (current: HubCallerContext) =
    let userId = current.UserIdentifier

    let wasAlreadyWaiting, old = waitingUsers.TryGetValue(userId)

    let success =
        if wasAlreadyWaiting then
            waitingUsers.TryUpdate(userId, current, old)
        else
            waitingUsers.TryAdd(userId, current)

    if not success then
        // in this cast existing user either had got paired in the meantime or disconnected,
        // the safest bet is to disconnect the current client and let it reconnect
        current.Abort()

    if success && wasAlreadyWaiting then
        old.Abort()

let reconnect
    (userConnections: ConcurrentDictionary<string, UserConnection>)
    (oldConnection: UserConnection)
    (current: HubCallerContext)
    =
    let userId = current.UserIdentifier

    match oldConnection with
    | ConnectedUser old ->
        // connection replacement
        let updated =
            userConnections.TryUpdate(userId, ConnectedUser current, oldConnection)

        if updated then
            // if successfully replaced then close the old one
            old.Abort()
        else
            // in case we couldn't replace a connection just close the current one
            current.Abort()

    | DisconnectedUser _ ->
        // reconnect
        let updated =
            userConnections.TryUpdate(userId, ConnectedUser current, oldConnection)

        if not updated then
            // in case we couldn't reconnect just close the current connection
            current.Abort()

let tryPair (waitingUsers: ConcurrentDictionary<string, HubCallerContext>) (current: HubCallerContext) =

    let userId = current.UserIdentifier
    let other = waitingUsers |> Seq.tryHead

    match other with
    | Some other when waitingUsers.ContainsKey userId = false && other.Key <> userId ->
        // TODO: pair
        true
    | _ -> false

type GameHub(logger: ILogger<GameHub>) =
    inherit Hub()

    static let games = ConcurrentDictionary<string, Tuple<string, string>>()

    static let waitingUsers = ConcurrentDictionary<string, HubCallerContext>()

    static let userConnections = ConcurrentDictionary<string, UserConnection>()

    override this.OnConnectedAsync() =
        logger.LogInformation(
            "New connection: {ConnectionId} for a user {UserId}",
            this.Context.ConnectionId,
            this.Context.UserIdentifier
        )

        task {
            let userId = this.Context.UserIdentifier
            let wasAlreadyConnected, oldConnection = userConnections.TryGetValue userId

            if wasAlreadyConnected then
                reconnect userConnections oldConnection this.Context
            elif not (tryPair waitingUsers this.Context) then
                addToWaiting waitingUsers this.Context

            return Task.FromResult
        }

    override this.OnDisconnectedAsync(ex) =
        // TODO: inject actual time provider
        let timeProvider = TimeProvider.System

        let userId = this.Context.ConnectionId

        let disconnectedUser =
            DisconnectedUser { DisconnectedAt = timeProvider.GetUtcNow() }

        let _, currentUser = userConnections.TryGetValue(userId)
        userConnections.TryUpdate(userId, disconnectedUser, currentUser) |> ignore

        // removes client if already waiting
        waitingUsers.TryRemove(userId) |> ignore

        logger.LogInformation(
            "User {UserId} disconnected; connectionId {ConnectionId}",
            userId,
            this.Context.ConnectionId
        )

        base.OnDisconnectedAsync(ex)
