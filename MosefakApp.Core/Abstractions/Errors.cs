namespace MosefakApp.Core.Abstractions
{
    public static class Errors
    {
        public const string PasswordMinLength = "minimum length of Password is 8 letters.";
        public const string PasswordRegExp = "Password must contain at least one lowercase, one uppercase, one alpha and one digit";
        public const string NameRegExp = "Name must be from letters A-Z,a-z and _ only";
    }
}
