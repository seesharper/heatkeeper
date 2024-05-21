using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server;

public record UpdateCommand : Command<Results<Ok, ProblemHttpResult>>
{

}
