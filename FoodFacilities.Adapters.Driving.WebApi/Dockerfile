#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV FoodFacility_ConnectionString /src/Database/Database.csv

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FoodFacilities.Adapters.Driving.WebApi/FoodFacilities.Adapters.Driving.WebApi.csproj", "FoodFacilities.Adapters.Driving.WebApi/"]
RUN dotnet restore "FoodFacilities.Adapters.Driving.WebApi/FoodFacilities.Adapters.Driving.WebApi.csproj"
COPY . .
WORKDIR "/src/FoodFacilities.Adapters.Driving.WebApi"
RUN dotnet build "FoodFacilities.Adapters.Driving.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodFacilities.Adapters.Driving.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodFacilities.Adapters.Driving.WebApi.dll"]