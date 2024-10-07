using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Validation;

public interface IValidator<in T> 
{
    Task Validate(T value);
}

