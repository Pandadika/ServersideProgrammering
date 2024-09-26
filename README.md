# Build and start docker container
`docker-compose up --build -d`
# Shutdown
`docker-compose down`
# Remove residual docker images and containers
`docker rm -f serverprog | docker rmi serverprog:v1`


init migration in webapi
`dotnet ef migrations add InitialCreate --context TodoContext --project .\WebApi\WebApi.csproj`

update db
`dotnet ef database update --context TodoContext --project .\WebApi\WebApi.csproj`