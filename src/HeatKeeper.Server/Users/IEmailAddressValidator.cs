using System;
using System.Net.Mail;
using HeatKeeper.Server.Exceptions;
using HeatKeeper.Server.Validation;

namespace HeatKeeper.Server.Users
{
    public interface IEmailValidator
    {
        void Validate(string email);
    }

    public class EmailValidator : IEmailValidator
    {
        public void Validate(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
            }
            catch (FormatException)
            {
                throw new ValidationFailedException($"The mail address '{email}' is not correctly formatted.");
            }
        }
    }


}
