# Tictactoe

An exciting multiplayer game where two players battle who is the best at filling the boxes with X's and O's.

## Purpose

This is a personal sandbox where I am trying to learn new concepts. While it does not fully represent me as a professional software developer, it serves as a good starting point for discussion.

## How to run

Prerequisites:
- .NET
- Node.js + pnpm

```
cd Tictactoe.AppHost
dotnet run
```

## How to start a game

- Client 1 creates a game by entering a code
- Client 2 joins the game by entering the same code

## The most notable projects in this repository

Tictactoe.Server
- The game server deployed in Azure App Service
- C# ASP.NET Core

tictactoe-client
- The game client deployed in Azure Static Web App
- Vite React SPA

Tictactoe.Engine
- The logic handling a game of tictactoe

Tictactoe.AppHost
- .NET Aspire orchestration

## Other projects

Tictactoe.App
- The game client using Blazor WASM

Tictactoe.Authentication
- OpenIddict authorization server

## Techstack

Frontend:

- Typescript
- React
- Redux
- React Router
- Material UI
- SignalR

Backend:

- C#
- ASP.NET Core
- ASP.NET Identity
- OpenIddict
- Entity Framework
- SignalR

## Other stuff

EF Core Migrations:

- Install

```
dotnet tool install --global dotnet-ef
```

- Run

```
cd Tictactoe.Authentication
dotnet ef database update
```

Run backend with authentication:

```
- Tictactoe.Server + Tictactoe.Authentication
```

Run frontend:

```
pnpm dev
```

Deploy bicep:

```
cd infra
az deployment group create --resource-group rg-tictactoe --template-file .\main.bicep
```

Further reading:

- https://learn.microsoft.com/en-us/azure/app-service/deploy-best-practices
- https://learn.microsoft.com/en-us/entra/workload-id/workload-identity-federation-create-trust-user-assigned-managed-identity
