FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Altair.BBS.Service/Altair.BBS.Service.csproj", "Altair.BBS.Service/"]
RUN dotnet restore "Altair.BBS.Service/Altair.BBS.Service.csproj"
COPY . .
WORKDIR "/src/Altair.BBS.Service"
RUN dotnet build "Altair.BBS.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Altair.BBS.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Altair.BBS.Service.dll"]
