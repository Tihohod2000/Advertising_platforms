# 1. Билдим проект
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем .csproj и восстанавливаем зависимости
COPY Advertising_platforms/Advertising_platforms.csproj .
RUN dotnet restore Advertising_platforms.csproj

# Копируем весь код
COPY Advertising_platforms/ .

# Паблишим
RUN dotnet publish Advertising_platforms.csproj -c Release -o /app/publish

# 2. Рантайм
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Advertising_platforms.dll"]
