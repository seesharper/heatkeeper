#!/usr/bin/env dotnet-script
#load "nuget:Dotnet.Build, 0.3.9"
#load "build-context.csx"

using static DotNet;

Test(IntegrationsTests);


