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
    test();
    testcoverage();
    await Docker.BuildAsync("bernhardrichter/heatkeeper", BuildContext.LatestTag, BuildContext.RepositoryFolder);
};

[DefaultStep]
[StepDescription("Deploys packages if we are on a tag commit in a secure environment.")]
AsyncStep deploy = async () =>
{
    await dockerImage();
    if (BuildEnvironment.IsSecure && BuildEnvironment.IsTagCommit)
    {
        await Docker.PushAsync("bernhardrichter/heatkeeper", BuildContext.LatestTag, BuildContext.RepositoryFolder);
    }

};

await StepRunner.Execute(Args);
return 0;
