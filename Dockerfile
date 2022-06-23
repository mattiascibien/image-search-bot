FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./image-search-bot/
WORKDIR /app/image-search-bot
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY ./ ./image-search-bot/
WORKDIR /app/image-search-bot
RUN dotnet publish -c Release -o out

# run application
FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/image-search-bot/out ./
CMD dotnet image-search-bot.dll