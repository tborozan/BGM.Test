FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BGM.Test.Web/BGM.Test.Web.csproj", "BGM.Test.Web/"]
RUN dotnet restore "BGM.Test.Web/BGM.Test.Web.csproj"
COPY . .
WORKDIR "/src/BGM.Test.Web"
RUN dotnet build "BGM.Test.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BGM.Test.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BGM.Test.Web.dll"]
