using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server;

public record UpdateCommand : Command<Results<Ok, ProblemHttpResult>>;

public record DeleteCommand : Command<NoContent>;