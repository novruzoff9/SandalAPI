# Use the official .NET Core 3.1 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

# Copy the csproj and restore as distinct layers
COPY ["IdentityServer/IdentityServer.csproj", "IdentityServer/"]
COPY ["Shared/Shared/Shared.csproj", "Shared/Shared/"]
RUN dotnet restore "IdentityServer/IdentityServer.csproj"

# Copy the remaining source code and build the application
COPY . .
WORKDIR "/src/IdentityServer"
RUN dotnet build "IdentityServer.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "IdentityServer.csproj" -c Release -o /app/publish

# Use the base image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]
