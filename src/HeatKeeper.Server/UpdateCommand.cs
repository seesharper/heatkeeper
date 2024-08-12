using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server;

public record PatchCommand : ProblemCommand<NoContent>;

public record DeleteCommand : ProblemCommand<NoContent>;