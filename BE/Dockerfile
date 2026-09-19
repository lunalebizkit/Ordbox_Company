
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Api/Ordbox.Api.csproj", "Ordbox.Api/"]
COPY ["Services/Ordbox.Services.csproj", "Ordbox.Services/"]
COPY ["Sdk/Ordbox.SDK.csproj", "Ordbox.Sdk/"]
COPY ["Domain/Ordbox.Domain.csproj", "Ordbox.Domain/"]

RUN dotnet restore "Ordbox.Api/Ordbox.Api.csproj"
COPY . .
WORKDIR "/src/Ordbox.Api"
RUN dotnet build "Ordbox.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ordbox.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordbox.Api.dll"]