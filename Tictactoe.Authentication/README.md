# Identity Scaffolding

- Add identity pages
- Add database tables

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-9.0&tabs=net-cli#scaffold-identity-into-a-blazor-project

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

