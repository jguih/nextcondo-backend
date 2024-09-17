#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["simplify-condo-api.csproj", "."]
#RUN dotnet restore "./simplify-condo-api.csproj"
#COPY . .
#WORKDIR "/src/."
#RUN dotnet build "./simplify-condo-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish

# RUN dotnet tool install --global dotnet-ef
# ENV PATH="$PATH:/root/.dotnet/tools"

# WORKDIR /app

# COPY . .

# RUN dotnet publish -c Release -o ./release /p:UseAppHost=false
# RUN dotnet ef migrations bundle -o ./release/bundle
# RUN chmod +x ./release/bundle

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS release
WORKDIR /app
RUN addgroup --system --gid 1001 nextcondoapi
RUN adduser --system --uid 1001 nextcondoapi
COPY --chown=nextcondoapi:nextcondoapi ./release ./
RUN chmod +x ./bundle
EXPOSE 8080
ENTRYPOINT ["dotnet", "App.dll"]