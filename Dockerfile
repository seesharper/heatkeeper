
ARG APP_VERSION=1.0.0
FROM alpine/git as clone-client-stage

WORKDIR /github

RUN git clone https://github.com/seesharper/heatkeeper.client


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-stage
ARG APP_VERSION
COPY src /src

WORKDIR /src/HeatKeeper.Server.Host

RUN echo "Building version " $APP_VERSION
RUN dotnet publish -c release -o /heatkeeper/app /property:Version=$APP_VERSION

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0

VOLUME [ "/db" ]

WORKDIR /app

COPY --from=build-stage /heatkeeper/app .

ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000

ENTRYPOINT [ "dotnet", "HeatKeeper.Server.Host.dll" ]