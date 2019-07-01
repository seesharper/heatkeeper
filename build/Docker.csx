#load "nuget:Dotnet.Build, 0.5.0"

public static class Docker
{
    public static void Build(string repository, string tag, string workingDirectory)
    {
        Command.Execute("docker", $@"build --rm -f ""Dockerfile"" -t {repository}:{tag} .", workingDirectory);
    }

    public static void Push(string repository, string tag, string workingDirectory)
    {
        var username = Environment.GetEnvironmentVariable("DOCKERHUB_USERNAME");
        var password = Environment.GetEnvironmentVariable("DOCKERHUB_PASSWORD");

        Command.Execute("docker", $"login --username {username} --password {password}", workingDirectory);
        Command.Execute("docker", $@"push {repository}:{tag}", workingDirectory);
    }
}