
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

RUN addgroup --system --gid 1000 appgroup \
    && adduser --system --uid 1000 --ingroup appgroup appuser

FROM mcr.microsoft.com/dotnet/sdk:10.0.401 AS build
WORKDIR /src
COPY ["Api/Ordbox.Api.csproj", "Api/"]
COPY ["Services/Ordbox.Services.csproj", "Services/"]
COPY ["Sdk/Ordbox.SDK.csproj", "Sdk/"]
COPY ["Domain/Ordbox.Domain.csproj", "Domain/"]

RUN dotnet restore "Api/Ordbox.Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Ordbox.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ordbox.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

RUN apt-get update && apt-get install -y openssl
COPY openssl.cnf /etc/ssl/openssl.cnf

COPY --from=publish /app/publish .

USER appuser
ENTRYPOINT ["dotnet", "Ordbox.Api.dll"]