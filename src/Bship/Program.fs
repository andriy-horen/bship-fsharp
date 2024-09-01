module Bship.Program

open System
open System.IdentityModel.Tokens.Jwt
open System.Security.Claims
open System.Text
open Bship.GameHub
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.IdentityModel.Tokens
open Microsoft.Extensions.Configuration

type User = { UserId: string; DisplayName: string }
type CreateTokenResponse = { Token: string }

type AuthorizationConfig = { JwtSecret: string }

let tokenIssuer = "https://bship.org/auth"

let tokenAudience = "https://bship.org"

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    
    builder.Configuration.AddEnvironmentVariables("BSHIP_") |> ignore

    let jwtSecret =
        builder.Configuration.GetRequiredSection("BSHIP_JWT_SECRET").Get<string>()
    
    let privateKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))

    builder.Services.AddCors().AddLogging().AddSignalR() |> ignore

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(fun options ->
            options.TokenValidationParameters <-
                TokenValidationParameters(
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenIssuer,
                    ValidAudience = tokenAudience,
                    IssuerSigningKey = privateKey
                ))
    |> ignore

    let app = builder.Build()

    app.MapGet("/", Func<string>(fun () -> "Hello World!")).WithMetadata(AllowAnonymousAttribute()) |> ignore
    // TODO: implement rate limiter per IP-address
    app.MapPost(
        "/auth/token",
        Func<CreateTokenResponse>(fun () ->
            let privateKey = privateKey
            let credentials = SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256)

            let tokenDescriptor =
                SecurityTokenDescriptor(
                    Subject =
                        ClaimsIdentity(
                            [ Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString())
                              Claim(JwtRegisteredClaimNames.Name, "john_doe") ]
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
                .WithOrigins("http://localhost:8081")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            |> ignore)
        .UseRouting()
        .UseAuthentication()
        .UseEndpoints(fun api -> api.MapHub<GameHub>("/game") |> ignore)
    |> ignore

    app.Run()
    0
