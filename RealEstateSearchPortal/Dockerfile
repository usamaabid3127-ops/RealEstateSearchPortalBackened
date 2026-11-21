FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RealEstateSearchPortal/RealEstateSearchPortal.csproj", "RealEstateSearchPortal/"]
RUN dotnet restore "RealEstateSearchPortal/RealEstateSearchPortal.csproj"
COPY . .
WORKDIR /src/RealEstateSearchPortal
RUN dotnet publish "RealEstateSearchPortal.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RealEstateSearchPortal.dll"]