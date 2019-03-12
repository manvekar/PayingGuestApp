FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 62316
EXPOSE 44375

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/FoodAppWeb/FoodAppWeb.csproj src/FoodAppWeb/
RUN dotnet restore src/FoodAppWeb/FoodAppWeb.csproj
COPY . .
WORKDIR /src/src/FoodAppWeb
RUN dotnet build FoodAppWeb.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FoodAppWeb.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FoodAppWeb.dll"]
