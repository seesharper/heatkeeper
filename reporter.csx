#! /usr/bin/env dotnet-script

#r "nuget:HeatKeeper.Reporter.Sdk, 0.15.0"

using HeatKeeper.Reporter.Sdk;

string apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImFkbWluQG5vLm9yZyIsImdpdmVuX25hbWUiOiJGaXJzdCBuYW1lIiwiZmFtaWx5X25hbWUiOiJMYXN0IE5hbWUiLCJyb2xlIjpbInJlcG9ydGVyIiwicmVwb3J0ZXIiXSwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiMSIsIm5iZiI6MTcwNzU5Mzk2MSwiZXhwIjoyMDIzMjEzMTYxLCJpYXQiOjE3MDc1OTM5NjF9.9pYFqn_f0jXQdUgz1sTRtnBQRCNOdEO9NLgmloSDR-c";
string heatKeeperUrl = "https://heatkeeper.no";
string hanReaderSerialPort = "/dev/ttyUSB0";

await new ReporterHost()
        .WithHeatKeeperEndpoint(heatKeeperUrl, apiKey)
        .AddReporter(new RTL433Reporter().AddSensor(Sensors.Acurite606TX).AddSensor(Sensors.AcuriteTower).AddSensor(Sensors.FineOffsetWH2).WithPublishInterval(new TimeSpan(0, 0, 30)))
        .AddReporter(new HANReporter().WithSerialPort(hanReaderSerialPort).WithPublishInterval(new TimeSpan(0, 0, 10)))
        .Start();
