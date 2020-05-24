dotnet ef migrations add InitialCreate --context SqlServerDataContext --output-dir AzdoAuthentication/Migrations/SqlServer -v

dotnet ef migrations remove --context SqlServerDataContext