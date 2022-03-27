FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bot/Bot.csproj", "./"]
RUN dotnet restore "Bot.csproj"
COPY "Bot/" .
WORKDIR "/src/."
RUN dotnet build "Bot.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "Bot.csproj" -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bot.dll"]