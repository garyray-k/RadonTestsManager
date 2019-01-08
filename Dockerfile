FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY RadonTestsManager/RadonTestsManager.csproj RadonTestsManager/
RUN dotnet restore RadonTestsManager/RadonTestsManager.csproj
COPY . .
WORKDIR /src/RadonTestsManager
RUN dotnet build RadonTestsManager.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish RadonTestsManager.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RadonTestsManager.dll"]
