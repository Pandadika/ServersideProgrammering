#!/bin/bash

# set -e

# #!/bin/bash
# set -e

# # Run database migrations for TodoContext
# dotnet ef database update --context TodoContext --project /app/Serversideprogrammering.App.csproj

# # Run database migrations for ApplicationDbContext
# dotnet ef database update --context ApplicationDbContext --project /app/Serversideprogrammering.App.csproj

# # Start the application
# exec dotnet Serversideprogrammering.App.dll

set -e

# Run database migrations for TodoContext
dotnet ef database update --context TodoContext --project /app/Serversideprogrammering.App.csproj

# Run database migrations for ApplicationDbContext
dotnet ef database update --context ApplicationDbContext --project /app/Serversideprogrammering.App.csproj

# Start the application
exec dotnet Serversideprogrammering.App.dll


