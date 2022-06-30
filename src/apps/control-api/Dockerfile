FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["control-api.csproj", "./"]
RUN dotnet restore "control-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "control-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "control-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "control-api.dll"]
