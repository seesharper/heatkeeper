FROM microsoft/dotnet:2.1.500-sdk

COPY src /src

WORKDIR /src


RUN dotnet publish -c release -o /app

EXPOSE 80

ENTRYPOINT [ "dotnet", "/app/HeatKeeper.Server.Host.dll" ]