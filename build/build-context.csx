#load "nuget:Dotnet.Build, 0.6.0"
using System.Xml.Linq;
using static FileUtils;

var owner = "seesharper";
var projectName = "HeatKeeper";
var root = FileUtils.GetScriptFolder();

var rootFolder = Path.GetFullPath(Path.Combine(root, ".."));

var version = XDocument.Load(Path.Combine(rootFolder, "src", "HeatKeeper.Server.Host", "HeatKeeper.Server.Host.csproj")).Descendants("Version").Single().Value;

var dockerRepository = "bernhardrichter/heatkeeper";

var artifactsFolder = CreateDirectory(root, "Artifacts");
var gitHubArtifactsFolder = CreateDirectory(artifactsFolder, "GitHub");
var nuGetArtifactsFolder = CreateDirectory(artifactsFolder, "NuGet");

var pathToReleaseNotes = Path.Combine(gitHubArtifactsFolder, "ReleaseNotes.md");

var coverageArtifactsFolder = CreateDirectory(artifactsFolder, "CodeCoverage");

var solutionFolder = Path.Combine(root, "..", "src");

var IntegrationsTests = Path.Combine(solutionFolder, "HeatKeeper.Server.WebApi.Tests");