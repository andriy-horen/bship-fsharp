module Bship.Program

open System
open System.IdentityModel.Tokens.Jwt
open System.Security.Claims
open System.Text
open System.Threading.Tasks
open Bship.GameHub
open Bship.GameMatchingService
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.IdentityModel.Tokens
open Microsoft.Extensions.Configuration

type User = { UserId: string; DisplayName: string }

type GenerateTokenResponse = { Token: string }

let tokenIssuer = "https://bship.org/auth"

let tokenAudience = "https://bship.org"

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Configuration.AddEnvironmentVariables("BSHIP_") |> ignore

    let jwtSecret =
        builder.Configuration.GetRequiredSection("BSHIP_JWT_SECRET").Get<string>()

    let privateKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))

    builder.Services
        .AddCors()
        .AddLogging()
        .AddAuthorization()
        // TODO: Provide Keep-Alive interval from the configuration
        .AddSignalR(fun options -> options.KeepAliveInterval <- TimeSpan.FromMilliseconds(5_000))
    |> ignore

    builder.Services
        .AddHostedService<GameMatchingService>()
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(fun options ->
            options.Authority <- tokenIssuer
            options.Audience <- tokenAudience

            options.TokenValidationParameters <-
                TokenValidationParameters(
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenIssuer,
                    ValidAudience = tokenAudience,
                    IssuerSigningKey = privateKey
                )

            options.Events <-
                JwtBearerEvents(
                    OnMessageReceived =
                        (fun context ->
                            let accessToken = context.Request.Query["access_token"]
                            let path = context.HttpContext.Request.Path
                            // TODO: path is quite generic could potentially match some other route
                            if String.IsNullOrEmpty accessToken = false && path.StartsWithSegments("/game") then
                                context.Token <- accessToken

                            Task.CompletedTask)
                ))
    |> ignore

    let app = builder.Build()

    app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore
    // TODO: implement rate limiter
    app.MapPost(
        "/auth/token",
        Func<GenerateTokenResponse>(fun () ->
            let privateKey = privateKey
            let credentials = SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256)

            let tokenDescriptor =
                SecurityTokenDescriptor(
                    Subject =
                        ClaimsIdentity(
                            [ Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString())
                              Claim(JwtRegisteredClaimNames.Name, "player") ]
                        ),
                    Issuer = tokenIssuer,
                    Audience = tokenAudience,
                    Expires = DateTime.UtcNow.AddDays 30,
                    SigningCredentials = credentials
                )

            let handler = JwtSecurityTokenHandler()
            let token = handler.CreateToken(tokenDescriptor)
            let jwtToken = handler.WriteToken(token)

            { Token = jwtToken })
    )
    |> ignore

    app
        .UseCors(fun cors ->
            cors
                .WithOrigins([| 4200 |] |> Array.map (fun port -> $"http://localhost:{port}"))
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            |> ignore)
        .UseRouting()
        .UseAuthentication()
        .UseAuthorization()
        .UseEndpoints(fun api -> api.MapHub<GameHub>("/game").RequireAuthorization() |> ignore)
    |> ignore

    app.Run()
    0
