# See https://aka.ms/customizecontainer to learn how to customize your debug container
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
# Base image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# Specifies the user that should be switched to when running the image
USER $APP_UID

# Sets the working directory inside the container
WORKDIR /app

# Exposes HTTP port
EXPOSE 8080

# Exposes HTTPS port
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Build configuration can be overridden through docker build command line argument
ARG BUILD_CONFIGURATION=Release

# Sets the working directory
WORKDIR /src

# Copy the project file
COPY ["ProductsMicroService.API/ProductsMicroService.API.csproj", "ProductsMicroService.API/"]

# Restore NuGet packages
RUN dotnet restore "./ProductsMicroService.API/ProductsMicroService.API.csproj"

# Copy the remaining source code
COPY . .

# Move into the API project directory
WORKDIR "/src/ProductsMicroService.API"

# Build the application
RUN dotnet build "./ProductsMicroService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build


# This stage is used to publish the service project
# The published output will be copied to the final stage
FROM build AS publish

# Build configuration
ARG BUILD_CONFIGURATION=Release

# Publish the application
RUN dotnet publish "./ProductsMicroService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# This stage is used in production or when running from VS in regular mode
# Default when not using the Debug configuration
FROM base AS final

# Sets the working directory
WORKDIR /app

# Copy published files from the publish stage
COPY --from=publish /app/publish .

ENV MYSQL_HOST=localhost 
ENV MYSQL_PASSWORD=admin

# Start the application
ENTRYPOINT ["dotnet", "ProductsMicroService.API.dll"]