using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server;

public record PostCommand : ProblemCommand<Created<ResourceId>>;


public record ProblemCommand<TResult> : Command<Results<TResult, ProblemHttpResult>>, IProblemCommand where TResult : IResult
{
    public void SetProblemResult(string detail, int statusCode) => SetResult(TypedResults.Problem(detail, statusCode: statusCode));
}


public interface IProblemCommand
{
    void SetProblemResult(string detail, int statusCode);
}
