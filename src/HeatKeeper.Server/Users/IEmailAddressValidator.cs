using System.Net.Mail;

namespace HeatKeeper.Server.Users;

public interface IEmailValidator
{
    bool Validate(string email);
}

public class EmailValidator : IEmailValidator
{
    public bool Validate(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
        }
        catch (FormatException)
        {
            return false;
        }
        return true;
    }
}
