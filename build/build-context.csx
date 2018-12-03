#load "nuget:Dotnet.Build, 0.3.9"
var owner = "seesharper";
var projectName = "heatkeeper";
var root = FileUtils.GetScriptFolder();

var solutionFolder = Path.Combine(root, "..", "src");

var IntegrationsTests = Path.Combine(solutionFolder, "HeatKeeper.Server.WebApi.Tests");