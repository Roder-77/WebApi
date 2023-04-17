#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# visual studio template
#FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["WebApi/WebApi.csproj", "WebApi/"]
#COPY ["Libraries/Models/Models.csproj", "Libraries/Models/"]
#COPY ["Libraries/Common/Common.csproj", "Libraries/Common/"]
#COPY ["Libraries/Services/Services.csproj", "Libraries/Services/"]
#RUN dotnet restore "WebApi/WebApi.csproj"
#COPY . .
#WORKDIR "/src/WebApi"
#RUN dotnet build "WebApi.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "WebApi.dll"]


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
# Copy everything
COPY . ./
# Restore as distinct layers
RUN donet restore
# Build and publish a release
RUN donet publish -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]

EXPOSE 80
EXPOSE 443
