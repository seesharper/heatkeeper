namespace HeatKeeper.Server.Host.Users
{
    public class ChangePasswordRequest
    {
        public ChangePasswordRequest(string oldPassword, string newPassword, string confirmedPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
            ConfirmedPassword = confirmedPassword;
        }

        public string OldPassword { get; }
        public string NewPassword { get; }
        public string ConfirmedPassword { get; }
    }
}