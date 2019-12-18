using System;
using System.Collections.Generic;

namespace HeatKeeper.Server.Validation
{
    public class ValidationFailedException : Exception
    {
        public ValidationFailedException(string message) :
            this(new List<ValidationError>() { new ValidationError(message) })
        {

        }


        public ValidationFailedException(ICollection<ValidationError> validationErrors)
        {
            ValidationErrors = validationErrors;
        }

        public ICollection<ValidationError> ValidationErrors { get; }
    }
}
