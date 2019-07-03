
#r "nuget:SimpleExec, 5.0.1"
#load "Command2.csx"
using static SimpleExec.Command;

public static class Docker
{
    public static void Build(string repository, string tag, string workingDirectory)
    {
        // Command.Execute("docker", $@"build --rm -f ""Dockerfile"" -t {repository}:{tag} .", workingDirectory);
    }

    public static async Task BuildAsync(string repository, string tag, string workingDirectory)
    {
        await Command2.ExecuteAsync("docker", $@"build --rm -f ""Dockerfile"" -t {repository}:{tag} .", workingDirectory);
        //await RunAsync("docker", $@"build --rm -f ""Dockerfile"" -t {repository}:{tag} .", workingDirectory);
    }

    public static void Push(string repository, string tag, string workingDirectory)
    {
        var username = Environment.GetEnvironmentVariable("DOCKERHUB_USERNAME");
        var password = Environment.GetEnvironmentVariable("DOCKERHUB_PASSWORD");

        // Command.Execute("docker", $"login --username {username} --password {password}", workingDirectory);
        // Command.Execute("docker", $@"push {repository}:{tag}", workingDirectory);
    }
}