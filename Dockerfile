FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Packages.props .
COPY Directory.Build.props .
COPY SchaerbeekMunicipality.slnx .
COPY src/ src/

RUN dotnet publish src/SchaerbeekMunicipality.Web/SchaerbeekMunicipality.Web.csproj \
    -c Release \
    -o /app/publish \
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
