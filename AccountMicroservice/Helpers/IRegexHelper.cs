namespace AccountMicroservice.Helpers
{
    public interface IRegexHelper
    {
        public bool IsValidEmail(string email);
        public bool IsValidPassword(string password);
    }
}