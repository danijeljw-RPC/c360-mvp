FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Cinturon360.Mock.Shared/Cinturon360.Mock.Shared.csproj src/Cinturon360.Mock.Shared/
COPY src/Cinturon360.Mock.Web/Cinturon360.Mock.Web.csproj src/Cinturon360.Mock.Web/
RUN dotnet restore src/Cinturon360.Mock.Web/Cinturon360.Mock.Web.csproj
COPY src/Cinturon360.Mock.Shared/ src/Cinturon360.Mock.Shared/
COPY src/Cinturon360.Mock.Web/ src/Cinturon360.Mock.Web/
RUN dotnet publish src/Cinturon360.Mock.Web/Cinturon360.Mock.Web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cinturon360.Mock.Web.dll"]
