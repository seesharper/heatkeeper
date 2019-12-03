namespace HeatKeeper.Server.Validation
{
    public class ValidationError
    {
        public ValidationError(string memberName, string errorMessage)
        {
            MemberName = memberName;
            ErrorMessage = errorMessage;
        }

        public string MemberName { get; }
        public string ErrorMessage { get; }
    }
}
