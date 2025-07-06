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

## for windows

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## for ubuntu

```
dotnet tool run dotnet-ef migrations add InitialCreate
dotnet tool run dotnet-ef database update
```

# access token

```
http://localhost:5138/api/getaccesstoken

{
    "username": "admin",
    "password": "admin123"
}

http://localhost:5138/api/SampleEnityBooks/1

postman
bulk edit

Authorization:Bearer token
```
