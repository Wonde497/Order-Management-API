FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderManagementAPI.csproj", "./"]
RUN dotnet restore "OrderManagementAPI.csproj"
COPY . .
RUN dotnet build "OrderManagementAPI.csproj" -c Release -o /app/build
RUN dotnet publish "OrderManagementAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "OrderManagementAPI.dll"]