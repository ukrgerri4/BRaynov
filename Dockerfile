#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /source
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY ["source/src/WebUI/*.csproj", "source/WebUI/"]
COPY ["source/src/WebApi/*.csproj", "source/WebApi/"]
COPY ["source/src/Application/*.csproj", "source/Application/"]
COPY ["source/src/Infrastructure/*.csproj", "source/Infrastructure/"]
COPY ["source/src/Domain/*.csproj", "source/Domain/"]
RUN dotnet restore "source/WebUI/WebUI.csproj"
RUN dotnet restore "source/WebApi/WebApi.csproj"
COPY . .
WORKDIR "/source/src/WebUI"
RUN dotnet build "WebUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebUI.csproj" -c Release -o /app/publish

WORKDIR "/source/src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebUI.dll"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
