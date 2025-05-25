FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Gateway/Gateway.Web/Gateway.Web.csproj", "Gateway/Gateway.Web/"]

COPY . .
WORKDIR "/src/Gateway/Gateway.Web"

RUN dotnet restore "Gateway.Web.csproj"

RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS publish
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Gateway.Web.dll"]