namespace MosefakApp.Infrastructure.constants
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";
        public const string GetAllUsers = "Users:GetAll";
        public const string GetUserById = "Users:GetById";
        public const string AddUser = "Users:Add";
        public const string EditUser = "Users:Edit";
        public const string DeleteUser = "Users:Delete";
        public const string UnLockUser = "Users:UnLock";

        public const string GetAllRoles = "Roles:GetAll";
        public const string GetRoleById = "Roles:GetById";
        public const string AddRole = "Roles:Add";
        public const string EditRole = "Roles:Edit";
        public const string DeleteRole = "Roles:Delete";

        public static IList<string> GetPermissions()
        {
            return typeof(Permissions).GetFields().Select(f => f.GetValue(f) as string).ToList()!;
        }
    }
}
