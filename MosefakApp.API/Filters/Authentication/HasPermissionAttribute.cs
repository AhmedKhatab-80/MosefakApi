namespace MosefakApp.API.Filters.Authentication
{
    public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
    {

    }
}
