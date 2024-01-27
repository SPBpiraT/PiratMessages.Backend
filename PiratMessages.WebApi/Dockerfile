#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PiratMessages.WebApi/PiratMessages.WebApi.csproj", "PiratMessages.WebApi/"]
COPY ["PiratMessages.Application/PiratMessages.Application.csproj", "PiratMessages.Application/"]
COPY ["PiratMessages.Domain/PiratMessages.Domain.csproj", "PiratMessages.Domain/"]
COPY ["PiratMessages.Persistence/PiratMessages.Persistence.csproj", "PiratMessages.Persistence/"]
RUN dotnet restore "PiratMessages.WebApi/PiratMessages.WebApi.csproj"
COPY . .
WORKDIR "/src/PiratMessages.WebApi"
RUN dotnet build "PiratMessages.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PiratMessages.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PiratMessages.WebApi.dll"]