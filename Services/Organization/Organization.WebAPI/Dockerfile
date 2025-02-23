# Use the official .NET 8 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_ENVIRONMENT=Production

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj and restore as distinct layers
COPY ["Services/Organization/Organization.WebAPI/Organization.WebAPI.csproj", "Services/Organization/Organization.WebAPI/"]
COPY ["Services/Organization/Organization.Application/Organization.Application.csproj", "Services/Organization/Organization.Application/"]
COPY ["Services/Organization/Organization.Infrastructure/Organization.Infrastructure.csproj", "Services/Organization/Organization.Infrastructure/"]
COPY ["Services/Organization/Organization.Domain/Organization.Domain.csproj", "Services/Organization/Organization.Domain/"]
COPY ["Shared/Shared/Shared.csproj", "Shared/Shared/"]
RUN dotnet restore "Services/Organization/Organization.WebAPI/Organization.WebAPI.csproj"

# Copy the remaining source code and build the application
COPY . .
WORKDIR "/src/Services/Organization/Organization.WebAPI"
RUN dotnet build "Organization.WebAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Organization.WebAPI.csproj" -c Release -o /app/publish

# Use the base image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Organization.WebAPI.dll"]
