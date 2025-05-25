FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FileAnalysis/FileAnalysis.Web/FileAnalysis.Web.csproj", "FileAnalysis/FileAnalysis.Web/"]
COPY ["FileAnalysis/FileAnalysis.UseCases/FileAnalysis.UseCases.csproj", "FileAnalysis/FileAnalysis.UseCases/"]
COPY ["FileAnalysis/FileAnalysis.Domain/FileAnalysis.Domain.csproj", "FileAnalysis/FileAnalysis.Domain/"]
COPY ["FileAnalysis/FileAnalysis.Infrastructure/FileAnalysis.Infrastructure.csproj", "FileAnalysis/FileAnalysis.Infrastructure/"]

COPY . .
WORKDIR "/src/FileAnalysis/FileAnalysis.Web"

RUN dotnet restore "FileAnalysis.Web.csproj"

RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS publish
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "FileAnalysis.Web.dll"]