# Build and start docker container
`docker-compose up --build -d`
# Shutdown
`docker-compose down`
# Remove residual docker images and containers
`docker rm -f serverprog | docker rmi serverprog:v1`