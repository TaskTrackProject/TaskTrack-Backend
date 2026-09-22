FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["QE190046_SE19B_Ass1_BE(1).sln", "."]
COPY ["TaskTrack.API/TaskTrack.API.csproj", "TaskTrack.API/"]
COPY ["TaskTrack.Repo/TaskTrack.Repo.csproj", "TaskTrack.Repo/"]
COPY ["TaskTrack.Service/TaskTrack.Service.csproj", "TaskTrack.Service/"]
RUN dotnet restore "TaskTrack.API/TaskTrack.API.csproj"

COPY . .
RUN dotnet publish "TaskTrack.API/TaskTrack.API.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskTrack.API.dll"]