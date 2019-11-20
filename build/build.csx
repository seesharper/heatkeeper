#load "nuget:Dotnet.Build, 0.9.0"
#load "nuget:dotnet-steps, 0.0.2"

BuildContext.CodeCoverageThreshold = 30;

[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () => DotNet.TestWithCodeCoverage();

[StepDescription("Runs all the tests for all target frameworks")]
Step test = () => DotNet.Test();

[StepDescription("Creates the NuGet packages")]
AsyncStep dockerImage = async () =>
{
    // test();
    // testcoverage();
    await Docker.BuildAsync("bernhardrichter/heatkeeper", "v1.0.0", BuildContext.RepositoryFolder);
};

[DefaultStep]
[StepDescription("Deploys packages if we are on a tag commit in a secure environment.")]
AsyncStep deploy = async () =>
{
    await dockerImage();
    await Artifacts.Deploy();
};

await StepRunner.Execute(Args);
return 0;
