using System;
using System.Text.RegularExpressions;
using HeatKeeper.Server.Exceptions;
using HeatKeeper.Server.Validation;

namespace HeatKeeper.Server.Authentication
{
    public class PasswordPolicy : IPasswordPolicy
    {
        // https://stackoverflow.com/questions/16945376/regex-for-complex-password-validation

        public void Apply(string password, string confirmedPassword)
        {
            ValidatePassword(password, confirmedPassword);
        }

        private void ValidatePassword(string password, string confirmedPassword)
        {
            if (!string.Equals(password, confirmedPassword))
            {
                throw new ValidationFailedException("The password does not match confirmed password");
            }

            var input = password;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ValidationFailedException("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,64}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                throw new ValidationFailedException("Password should contain at least one lower case letter");
            }
            if (!hasUpperChar.IsMatch(input))
            {
                throw new ValidationFailedException("Password should contain at least one upper case letter");
            }

            if (!hasMiniMaxChars.IsMatch(input))
            {
                throw new ValidationFailedException("Password should not be less than 8 or greater than 64 characters");
            }
            if (!hasNumber.IsMatch(input))
            {
                throw new ValidationFailedException("Password should contain at least one numeric value");
            }

            if (!hasSymbols.IsMatch(input))
            {
                throw new ValidationFailedException("Password should contain at least one special case characters");
            }
        }

    }



}
