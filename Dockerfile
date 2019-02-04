FROM microsoft/dotnet:2.2-sdk

COPY src /src

WORKDIR /src


RUN dotnet publish -c release -o /heatkeeper/app

VOLUME [ "/db" ]

EXPOSE 80

ENTRYPOINT [ "dotnet", "/heatkeeper/app/HeatKeeper.Server.Host.dll" ]