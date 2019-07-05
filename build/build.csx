#!/usr/bin/env dotnet-script
#load "nuget:Dotnet.Build, 0.6.0"
#load "nuget:github-changelog, 0.1.5"
#load "nuget:dotnet-steps, 0.0.1"
#load "build-context.csx"
#load "Docker.csx"

using static ChangeLog;
using static DotNet;
using static ReleaseManagement;

[StepDescription("Runs all the tests")]
Step test = () => Test(IntegrationsTests);

[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () =>
{
    DotNet.TestWithCodeCoverage(projectName, IntegrationsTests, coverageArtifactsFolder, targetFramework: "netcoreapp2.2", threshold: 10);
};

[StepDescription("Builds the docker image using the latest git tag.")]
AsyncStep dockerimage = async () =>
{
    await Docker.BuildAsync(dockerRepository, version, rootFolder);
};


[DefaultStep]
[StepDescription("Deploys a new container image if we are on a tag commit in a secure environment.")]
AsyncStep deploy = async () =>
{
    test();
    testcoverage();
    await dockerimage();
    if (!BuildEnvironment.IsSecure)
    {
        Logger.Log("Deployment can only be done in a secure environment");
        return;
    }

    await CreateReleaseNotes();

    if (Git.Default.IsTagCommit())
    {
        Git.Default.RequireCleanWorkingTree();
        await ReleaseManagerFor(owner, projectName, BuildEnvironment.GitHubAccessToken)
        .CreateRelease(Git.Default.GetLatestTag(), pathToReleaseNotes, Array.Empty<ReleaseAsset>());
        await Docker.PushAsync(dockerRepository, version, rootFolder);
    }
};

await ExecuteSteps(Args);


private async Task CreateReleaseNotes()
{
    Logger.Log("Creating release notes");
    var generator = ChangeLogFrom(owner, projectName, BuildEnvironment.GitHubAccessToken).SinceLatestTag();
    if (!Git.Default.IsTagCommit())
    {
        generator = generator.IncludeUnreleased();
    }
    await generator.Generate(pathToReleaseNotes, FormattingOptions.Default.WithPullRequestBody());
}