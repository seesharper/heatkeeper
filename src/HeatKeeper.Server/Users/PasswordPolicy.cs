using System;
using System.Text.RegularExpressions;
using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Users
{
    public class PasswordPolicy : IPasswordPolicy
    {
        // https://stackoverflow.com/questions/16945376/regex-for-complex-password-validation

        public void Apply(string passwordCandidate)
        {
            ValidatePassword(passwordCandidate);
        }

        private void ValidatePassword(string password)
        {
            var input = password;
            string errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,64}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                throw new HeatKeeperSecurityException("Password should contain at least one lower case letter");
            }
            if (!hasUpperChar.IsMatch(input))
            {
                throw new HeatKeeperSecurityException("Password should contain at least one upper case letter");
            }

            if (!hasMiniMaxChars.IsMatch(input))
            {
                throw new HeatKeeperSecurityException("Password should not be less than 8 or greater than 64 characters");
            }
            if (!hasNumber.IsMatch(input))
            {
                throw new HeatKeeperSecurityException("Password should contain At least one numeric value");
            }

            if (!hasSymbols.IsMatch(input))
            {
                throw new HeatKeeperSecurityException("Password should contain at least one special case characters");
            }
        }

    }



}