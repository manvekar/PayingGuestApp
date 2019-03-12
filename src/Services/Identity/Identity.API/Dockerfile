#define argument variable to define the docker nodeJs image required 
ARG NODE_IMAGE=node:8.11
# FROM microsoft/dotnet:2.2-runtime AS base
FROM microsoft/dotnet:2.2.0-aspnetcore-runtime AS base	
WORKDIR /app
EXPOSE 80

#intermediate image to prepare code build
FROM microsoft/dotnet:2.2-sdk AS dotnet-build	
WORKDIR /src

#intermediate image to prepare nodeJS and install bower and install required bower components
FROM ${NODE_IMAGE} as node-build	
WORKDIR /web
COPY src/Services/Identity/Identity.API .
#orignal version 1.8.4
RUN npm install -g bower@1.8.8	
RUN bower install --allow-root

#copy UI code from node image to build container UI folder
FROM dotnet-build as build		
WORKDIR /src/src/Services/Identity/Identity.API/wwwroot
COPY --from=node-build /web/wwwroot .
WORKDIR /src
#copy all the contents from root project folder to /src of the container folder
COPY . .
WORKDIR /src/src/Services/Identity/Identity.API
#dotnet restore nuget packages before build starts
RUN dotnet restore -nowarn:msb3202,nu1503
#build dotnet code
RUN dotnet build --no-restore -c Release -o /app 

FROM build AS publish	
RUN dotnet publish Identity.API.csproj -c Release -o /app

#final container preperation for generating build container defenition
FROM base AS final	
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Identity.API.dll"]
