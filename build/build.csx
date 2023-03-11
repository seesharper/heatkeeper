#load "nuget:Dotnet.Build, 0.16.1"
#load "nuget:dotnet-steps, 0.0.2"

BuildContext.CodeCoverageThreshold = 30;

[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () =>
{
    Command.Execute("docker-compose", $"-f \"{Path.Combine(BuildContext.RepositoryFolder, "docker-compose-dev.yml")}\" up -d");
    DotNet.TestWithCodeCoverage();
    Command.Execute("docker-compose", $"-f \"{Path.Combine(BuildContext.RepositoryFolder, "docker-compose-dev.yml")}\" down");
};

[StepDescription("Runs all the tests for all target frameworks")]
Step test = () =>
{
    Command.Execute("docker-compose", $"-f \"{Path.Combine(BuildContext.RepositoryFolder, "docker-compose-dev.yml")}\" up -d");
    DotNet.Test();
    Command.Execute("docker-compose", $"-f \"{Path.Combine(BuildContext.RepositoryFolder, "docker-compose-dev.yml")}\" down");
};

[StepDescription("Creates the Docker image")]
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

    if (BuildEnvironment.IsSecure && BuildEnvironment.IsTagCommit)
    {
        await dockerImage();
        await Docker.PushAsync("bernhardrichter/heatkeeper", BuildContext.LatestTag, BuildContext.BuildFolder);
    }

    await Artifacts.Deploy();
};

await StepRunner.Execute(Args);
return 0;
