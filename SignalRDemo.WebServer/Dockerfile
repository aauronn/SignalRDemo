FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY SignalRDemo.WebServer.sln ./
COPY SignalRDemo.WebServer/SignalRDemo.WebServer.csproj SignalRDemo.WebServer/
RUN dotnet restore SignalRDemo.WebServer/SignalRDemo.WebServer.csproj -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/SignalRDemo.WebServer
RUN dotnet build SignalRDemo.WebServer.csproj -c Release -o /app 

FROM build AS publish
RUN dotnet publish SignalRDemo.WebServer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SignalRDemo.WebServer.dll"]
