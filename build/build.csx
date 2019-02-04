#!/usr/bin/env dotnet-script
#load "nuget:Dotnet.Build, 0.3.9"
#load "nuget:dotnet-steps, 0.0.1"
#load "build-context.csx"

using static DotNet;

Step test = () => Test(IntegrationsTests);

await ExecuteSteps(Args);
