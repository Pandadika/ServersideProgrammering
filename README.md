# ⚠VIGTIGT⚠
Kræver et certificate placeret i  `%USERPROFILE%\.aspnet\https`

## HowTo
cmd
``` cmd
dotnet dev-certs https -ep %USERPROFILE%/.aspnet/https/cert.pfx -p Passw0rd
```
pws
``` powershell
dotnet dev-certs https -ep $env:USERPROFILE/.aspnet/https/cert.pfx -p Passw0rd
``` 

Sæt password i docker-compose.yml
eg.
``` yaml
(...)
    environment:
      ASPNETCORE_URLS: "https://+;http://+" 
      ASPNETCORE_HTTPS_PORT: 7001
      ASPNETCORE_Kestrel__Certificates__Default__Path: /https/cert.pfx
      ASPNETCORE_Kestrel__Certificates__Default__Password: "Passw0rd"
      ASPNETCORE_ENVIRONMENT: Development
      ApiBaseAddress: "https://webapi:443" 
(...)
```

# Byg og start docker container
`docker-compose up --build -d`
# Shutdown
`docker-compose down`
# Fjern resterende docker images og containers
`docker rm -f webapi | docker rm -f serverprog | docker rmi serverprog:v1 | docker rmi webapi:v1`
# Fjern persistent volumes 
`docker-compose down --volumes`