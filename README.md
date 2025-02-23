# TicTacToe

Frontend:
- Typescript
- React
- Redux
- React Router
- Material UI
- SignalR

Backend:
- C#, F#
- ASP.NET Core
- ASP.NET Core Identity
- OpenIddict
- Entity Framework
- SignalR

EF Core Migrations:

- Install
```
> dotnet tool install --global dotnet-ef
```
- Run
```
> cd TicTacToe.Authentication
> dotnet ef database update
```

Run backend:
```
- TicTacToe.Server + TicTacToe.Authentication
```

Run frontend:
```
> pnpm dev
```

Deploy bicep:
```
> cd infra
> az deployment group create --resource-group rg-tictactoe --template-file .\main.bicep
```

Further reading:
- https://learn.microsoft.com/en-us/azure/app-service/deploy-best-practices
- https://learn.microsoft.com/en-us/entra/workload-id/workload-identity-federation-create-trust-user-assigned-managed-identity