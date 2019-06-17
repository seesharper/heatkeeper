
FROM alpine/git as clone-client-stage

WORKDIR /github

RUN git clone https://github.com/seesharper/heatkeeper.client

FROM node:10-alpine as client-build

WORKDIR /src

COPY --from=clone-client-stage github/heatkeeper.client .

RUN npm install
RUN npm run build

# Build stage
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-stage

COPY src /src

WORKDIR /src

RUN dotnet publish -c release -o /heatkeeper/app

# Runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2

VOLUME [ "/db" ]

WORKDIR /app

COPY --from=build-stage /heatkeeper/app .

EXPOSE 80

ENTRYPOINT [ "dotnet", "HeatKeeper.Server.Host.dll" ]