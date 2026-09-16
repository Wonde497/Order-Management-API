# Step 1: Use .NET 10 SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["OrderManagementAPI.csproj", "./"]
RUN dotnet restore "OrderManagementAPI.csproj"

# Copy remaining files and publish
COPY . .
RUN dotnet build "OrderManagementAPI.csproj" -c Release -o /app/build
RUN dotnet publish "OrderManagementAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Step 2: Use .NET 10 ASP.NET Runtime for running the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "OrderManagementAPI.dll"]