FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FileStorage/FileStorage.Web/FileStorage.Web.csproj", "FileStorage/FileStorage.Web/"]
COPY ["FileStorage/FileStorage.UseCases/FileStorage.UseCases.csproj", "FileStorage/FileStorage.UseCases/"]
COPY ["FileStorage/FileStorage.Domain/FileStorage.Domain.csproj", "FileStorage/FileStorage.Domain/"]
COPY ["FileStorage/FileStorage.Infrastructure/FileStorage.Infrastructure.csproj", "FileStorage/FileStorage.Infrastructure/"]

COPY . .
WORKDIR "/src/FileStorage/FileStorage.Web"

RUN dotnet restore "FileStorage.Web.csproj"

RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS publish
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "FileStorage.Web.dll"]