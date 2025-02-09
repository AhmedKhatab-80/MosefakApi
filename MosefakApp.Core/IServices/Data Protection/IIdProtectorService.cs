namespace MosefakApp.Core.IServices.Data_Protection
{
    public interface IIdProtectorService
    {
        string Protect(int id);
        int? UnProtect(string protectedId);
    }
}
