#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NotifyMe.API/NotifyMe.API.csproj", "NotifyMe.API/"]
COPY ["NotifyMe.Infrastructure/NotifyMe.Infrastructure.csproj", "NotifyMe.Infrastructure/"]
COPY ["NotifyMe.Core/NotifyMe.Core.csproj", "NotifyMe.Core/"]
RUN dotnet restore "NotifyMe.API/NotifyMe.API.csproj"
COPY . .
WORKDIR "/src/NotifyMe.API"
RUN dotnet build "NotifyMe.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotifyMe.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotifyMe.API.dll"]