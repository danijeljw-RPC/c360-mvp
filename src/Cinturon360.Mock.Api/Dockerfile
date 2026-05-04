FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Cinturon360.Mock.Shared/Cinturon360.Mock.Shared.csproj src/Cinturon360.Mock.Shared/
COPY src/Cinturon360.Mock.Api/Cinturon360.Mock.Api.csproj src/Cinturon360.Mock.Api/
RUN dotnet restore src/Cinturon360.Mock.Api/Cinturon360.Mock.Api.csproj
COPY src/Cinturon360.Mock.Shared/ src/Cinturon360.Mock.Shared/
COPY src/Cinturon360.Mock.Api/ src/Cinturon360.Mock.Api/
RUN dotnet publish src/Cinturon360.Mock.Api/Cinturon360.Mock.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY mock-data/ mock-data/
ENTRYPOINT ["dotnet", "Cinturon360.Mock.Api.dll"]
