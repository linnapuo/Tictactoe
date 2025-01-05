# ASP.NET Core Identity

- Add identity pages and tables

[Scaffolding to an existing project](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-9.0&tabs=net-cli#scaffold-identity-into-a-blazor-project)

```
dotnet tool install --global dotnet-aspnet-codegenerator
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools

dotnet aspnet-codegenerator identity --useDefaultUI

dotnet ef migrations add CreateIdentitySchema
dotnet ef database update
```

# OpenIddict

- Add authorization schema and endpoints

[OpenIddict samples](https://github.com/openiddict/openiddict-samples)

```
dotnet add package OpenIddict.AspNetCore
dotnet add package OpenIddict.EntityFrameworkCore
dotnet add package OpenIddict.Quartz
dotnet add package Quartz.Extensions.Hosting
```

```
dotnet ef migrations add CreateOpenIddictSchema
dotnet ef database update
```