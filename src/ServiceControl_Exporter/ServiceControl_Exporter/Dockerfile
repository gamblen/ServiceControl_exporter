#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app
EXPOSE 9002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ServiceControl_Exporter/ServiceControl_Exporter.csproj", "ServiceControl_Exporter/"]
RUN dotnet restore "ServiceControl_Exporter/ServiceControl_Exporter.csproj"
COPY . .
WORKDIR "/src/ServiceControl_Exporter"
RUN dotnet build "ServiceControl_Exporter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceControl_Exporter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceControl_Exporter.dll"]