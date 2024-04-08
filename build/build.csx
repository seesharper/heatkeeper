#load "nuget:Dotnet.Build, 0.22.0"
#load "nuget:dotnet-steps, 0.0.2"

BuildContext.CodeCoverageThreshold = 30;

var dockerImageName = "heatkeeper";
var TagVersion = BuildContext.LatestTag;


[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () =>
{
    if (File.Exists(Path.Combine(BuildContext.RepositoryFolder, "src/HeatKeeper.Server.WebApi.Tests/bin/Debug/net8.0/heatkeeper.db")))
    {
        File.Delete(Path.Combine(BuildContext.RepositoryFolder, "src/HeatKeeper.Server.WebApi.Tests/bin/Debug/net8.0/heatkeeper.db"));
    }
    if (File.Exists(Path.Combine(BuildContext.RepositoryFolder, "src/HeatKeeper.Server.WebApi.Tests/bin/release/net8.0/heatkeeper.db")))
    {
        File.Delete(Path.Combine(BuildContext.RepositoryFolder, "src/HeatKeeper.Server.WebApi.Tests/bin/release/net8.0/heatkeeper.db"));
    }
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
    await BuildContainer();

    if (BuildEnvironment.IsSecure && BuildEnvironment.IsTagCommit)
    {
        await PushContainer();
    }

    await Artifacts.Deploy();
};

AsyncStep BuildContainer = async () =>
{
    WriteLine($"Building container for version {TagVersion}");
    await Command.ExecuteAsync("docker", $"build --pull --rm --build-arg APP_VERSION={TagVersion} --no-cache -t bernhardrichter/{dockerImageName}:latest .", BuildContext.RepositoryFolder);
    await Command.ExecuteAsync("docker", $"tag bernhardrichter/{dockerImageName}:latest bernhardrichter/{dockerImageName}:{TagVersion}");
};

AsyncStep PushContainer = async () =>
{
    var username = Environment.GetEnvironmentVariable("DOCKERHUB_USERNAME");
    var password = Environment.GetEnvironmentVariable("DOCKERHUB_PASSWORD");
    await Command.ExecuteAsync("docker", $"login --username {username} --password {password}", BuildContext.RepositoryFolder);
    await Command.ExecuteAsync("docker", $@"push bernhardrichter/{dockerImageName}:{BuildContext.LatestTag}", BuildContext.RepositoryFolder);
    await Command.ExecuteAsync("docker", $@"push bernhardrichter/{dockerImageName}:latest", BuildContext.RepositoryFolder);
};


await StepRunner.Execute(Args);
return 0;
