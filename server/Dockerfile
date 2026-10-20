FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Security/Security.csproj Security/
COPY DataAccess/DataAccess.csproj DataAccess/
RUN dotnet restore Security/Security.csproj

COPY . .
RUN dotnet publish Security/Security.csproj -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /out ./

RUN mkdir -p /app/data && chown app:app /app/data

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=4000

USER app

EXPOSE 4000

ENTRYPOINT ["dotnet", "Security.dll"]
