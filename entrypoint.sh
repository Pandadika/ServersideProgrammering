#!/bin/bash

set -e

# Run database migrations for TodoContext
dotnet ef database update --context TodoContext

# Start the application
exec dotnet WebApp.dll
