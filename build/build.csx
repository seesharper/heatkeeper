#!/usr/bin/env dotnet-script
#load "nuget:Dotnet.Build, 0.5.0"
#load "nuget:dotnet-steps, 0.0.1"
#load "build-context.csx"

using static DotNet;

[StepDescription("Runs all the tests")]
Step test = () => Test(IntegrationsTests);

[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () =>
{
    DotNet.TestWithCodeCoverage(projectName, IntegrationsTests, coverageArtifactsFolder, targetFramework: "netcoreapp2.2", threshold: 10);
};

await ExecuteSteps(Args);
