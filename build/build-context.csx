#load "nuget:Dotnet.Build, 0.5.0"
using static FileUtils;

var owner = "seesharper";
var projectName = "HeatKeeper";
var root = FileUtils.GetScriptFolder();

var artifactsFolder = CreateDirectory(root, "Artifacts");
var gitHubArtifactsFolder = CreateDirectory(artifactsFolder, "GitHub");
var nuGetArtifactsFolder = CreateDirectory(artifactsFolder, "NuGet");

var coverageArtifactsFolder = CreateDirectory(artifactsFolder, "CodeCoverage");

var solutionFolder = Path.Combine(root, "..", "src");

var IntegrationsTests = Path.Combine(solutionFolder, "HeatKeeper.Server.WebApi.Tests");