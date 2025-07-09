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

# `MailHog`

```sh
# https://github.com/mailhog/MailHog/releases/tag/v1.0.1

# windows
https://github.com/mailhog/MailHog/releases/download/v1.0.1/MailHog_windows_amd64.exe

# linux
https://github.com/mailhog/MailHog/releases/download/v1.0.1/MailHog_linux_amd64
```

```sh
./MailHog_linux_amd64
```

## for ubuntu

```sh
dotnet tool run dotnet-ef migrations add InitialCreate
dotnet tool run dotnet-ef database update

# delete database

## install packages - ubuntu 20.04
curl https://packages.microsoft.com/keys/microsoft.asc | sudo tee /etc/apt/trusted.gpg.d/microsoft.asc
curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list | sudo tee /etc/apt/sources.list.d/mssql-release.list
sudo apt-get update
sudo apt-get install mssql-tools mssql-tools18 unixodbc-dev

## one line delete command
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P '123456' -Q "DROP DATABASE PRN232_Library;"
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
