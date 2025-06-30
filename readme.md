```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "ConnectionStrings": {
        "Default": "Server=SQLEXPRESS;database=PRN232_Library;uid=sa;password=a;TrustServerCertificate=True;"
    },
    "AllowedHosts": "*"
}
```

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```
