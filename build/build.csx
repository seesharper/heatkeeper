#load "nuget:Dotnet.Build, 0.9.3"
#load "nuget:dotnet-steps, 0.0.2"

BuildContext.CodeCoverageThreshold = 30;

[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () => DotNet.TestWithCodeCoverage();

[StepDescription("Runs all the tests for all target frameworks")]
Step test = () => DotNet.Test();

[StepDescription("Creates the NuGet packages")]
AsyncStep dockerImage = async () =>
{
    await Docker.BuildAsync("bernhardrichter/heatkeeper", BuildContext.LatestTag, BuildContext.RepositoryFolder);
};

[DefaultStep]
[StepDescription("Deploys packages if we are on a tag commit in a secure environment.")]
AsyncStep deploy = async () =>
{
    test();
    testcoverage();
    await dockerImage();
    if (BuildEnvironment.IsSecure && BuildEnvironment.IsTagCommit)
    {
        await Docker.PushAsync("bernhardrichter/heatkeeper", BuildContext.LatestTag, BuildContext.RepositoryFolder);
    }

    await Artifacts.Deploy();
};

await StepRunner.Execute(Args);
return 0;
