namespace MosefakApp.Core.Abstractions
{
    public static class RegexPatterns
    {
        public const string NamePattern = "[a-zA-Z_ ]+";
        public const string PasswordPattern = "^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$";
    }
}
