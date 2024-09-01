module Bship.GameHub

open System
open System.Collections.Concurrent
open System.Security.Claims
open System.Threading.Tasks
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Logging

let getUserId (context: HubCallerContext) =
    context.User.FindFirstValue(ClaimTypes.NameIdentifier)

let addOrUpdateWaitingClient
    (logger: ILogger)
    (waitingClients: ConcurrentDictionary<string, HubCallerContext>)
    (context: HubCallerContext)
    =
    let userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
    let inWaiting, existingContext = waitingClients.TryGetValue userId

    if inWaiting && existingContext.ConnectionId <> context.ConnectionId then
        logger.LogInformation(
            "Closing existing connection for user {UserId}; connectionId: {ConnectionId}",
            userId,
            existingContext.ConnectionId
        )

        existingContext.Abort()

    logger.LogInformation(
        "Adding user {UserId} to the waiting list; connectionId: {ConnectionId}",
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
        let userId = getUserId client

        if removed then
            logger.LogInformation(
                "Found waiting user {UserId}, removed for pairing; connectionId {ConnectionId}",
                userId,
                client.ConnectionId
            )

            Some client
        else
            logger.LogWarning(
                "Found waiting user {UserId}, failed to remove for pairing; connectionId: {ConnectionId}",
                userId,
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
    let currentUserId = getUserId current
    let otherUserId = getUserId other

    if added then
        logger.LogInformation(
            "Creating a group {GroupId} with two clients {User1}, {User2}",
            groupId,
            currentUserId,
            otherUserId
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

type GameHub(logger: ILogger<GameHub>) =
    inherit Hub()

    static let games =
        ConcurrentDictionary<string, Tuple<HubCallerContext, HubCallerContext>>()

    static let waitingClients = ConcurrentDictionary<string, HubCallerContext>()

    override this.OnConnectedAsync() =
        logger.LogDebug("New connection: {ConnectionId}", this.Context.ConnectionId)

        task {
            let groupId = Guid.NewGuid().ToString()

            let pairingTask =
                tryGetCurrentClient waitingClients this.Context
                |> Option.bind (fun _ -> tryGetWaitingClient logger waitingClients)
                |> Option.bind (tryPairClients logger this groupId games this.Context)

            match pairingTask with
            | Some pairClients -> do! pairClients
            | None -> addOrUpdateWaitingClient logger waitingClients this.Context
        }

    override self.OnDisconnectedAsync(ex) =
        // let connectionId = self.Context.ConnectionId
        //
        // if waitingClients.ContainsKey connectionId then
        //     waitingClients.TryRemove connectionId |> ignore
        //
        // let _, gameId = self.Context.Items.TryGetValue groupIdKey
        // // if connection has a gameId then disconnect both clients
        // if gameId :? string then
        //     let gameId = gameId :?> string
        //     let success, (client1, client2) = games.TryGetValue gameId
        //
        //     if success then
        //         client1.Abort()
        //         client2.Abort()
        //
        base.OnDisconnectedAsync(ex)
