FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Packages.props Directory.Build.props SchaerbeekMunicipality.slnx ./

# Copy project files first so restore is cached when only source changes.
COPY src/SchaerbeekMunicipality.Domain/SchaerbeekMunicipality.Domain.csproj \
     src/SchaerbeekMunicipality.Domain/
COPY src/SchaerbeekMunicipality.ServiceDefaults/SchaerbeekMunicipality.ServiceDefaults.csproj \
     src/SchaerbeekMunicipality.ServiceDefaults/
COPY src/SchaerbeekMunicipality.Infrastructure/SchaerbeekMunicipality.Infrastructure.csproj \
     src/SchaerbeekMunicipality.Infrastructure/
COPY src/SchaerbeekMunicipality.Application/SchaerbeekMunicipality.Application.csproj \
     src/SchaerbeekMunicipality.Application/
COPY src/SchaerbeekMunicipality.Api/SchaerbeekMunicipality.Api.csproj \
     src/SchaerbeekMunicipality.Api/
COPY src/SchaerbeekMunicipality.Web/SchaerbeekMunicipality.Web.csproj \
     src/SchaerbeekMunicipality.Web/

RUN dotnet restore src/SchaerbeekMunicipality.Web/SchaerbeekMunicipality.Web.csproj

COPY src/ src/

RUN dotnet publish src/SchaerbeekMunicipality.Web/SchaerbeekMunicipality.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# The aspnet image runs as a non-root user ($APP_UID) that cannot create /app/uploads at runtime.
RUN mkdir -p /app/uploads && chown $APP_UID /app/uploads

USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SchaerbeekMunicipality.Web.dll"]
