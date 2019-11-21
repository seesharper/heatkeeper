using AutoFixture.Kernel;
using System;
using System.Reflection;

namespace HeatKeeper.Server.WebApi.Tests.Customizations
{
    public class PasswordCustomization : ISpecimenBuilder
    {
        private const string Password = "aVe78!*PZ9&Lnqh1E4pG";

        public object Create(object request, ISpecimenContext context)
        {
            if (request is ParameterInfo parameter)
            {
                if (parameter.Name.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Password;
                }
            }

            return new NoSpecimen();
        }
    }
}
