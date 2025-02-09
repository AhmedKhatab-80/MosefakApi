namespace MosefakApi.Business.Services.Data_Protection
{
    public class IdProtectorService : IIdProtectorService
    {
        private readonly IDataProtector _protector;

        public IdProtectorService(IDataProtectionProvider protectionProvider)
        {
            _protector = protectionProvider.CreateProtector(purpose: "SecureId");
        }
        public string Protect(int id)
        {
            return _protector.Protect(id.ToString());
        }

        public int? UnProtect(string protectedId)
        {
            try
            {
                string decryptedId = _protector.Unprotect(protectedId);
                return int.Parse(decryptedId);
            }
            catch
            {
                return null; // Return null if decryption fails
            }
        }
    }
}
