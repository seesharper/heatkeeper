using AutoFixture;
using AutoFixture.Kernel;
using System;
using System.Reflection;
using System.Net.Mail;

namespace HeatKeeper.Server.WebApi.Tests.Customizations
{
    public class MailAddressCustomization : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is ParameterInfo parameter)
            {
                if (parameter.Name.Contains("email", StringComparison.InvariantCultureIgnoreCase))
                {
                    return context.Create<MailAddress>().Address;
                }
            }

            return new NoSpecimen();
        }
    }
}