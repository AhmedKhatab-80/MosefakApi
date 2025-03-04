namespace MosefakApp.Core.IServices.Role
{
    public interface IRoleService
    {
        Task<(IList<RoleResponse> responses,int totalPages)> GetRolesAsync(bool IncludeDeleted = false, int pageNumber = 1, int pageSize = 10);
        Task<(IList<RoleResponse> responses,int totalPages)> GetRolesWithPermissionsAsync(bool IncludeDeleted = false, int pageNumber = 1, int pageSize = 10);
        Task<RoleResponse> GetRoleByIdAsync(int id);
        Task<RoleResponse> GetRoleByIdWithPermissionsAsync(int id);
        Task<RoleResponse> AddRoleAsync(RoleRequest request);
        Task<RoleResponse> EditRoleAsync(int id, RoleRequest request);
        Task DeleteRoleAsync(int id);
        Task<bool> AssignPermissionToRoleAsync(string roleId, AssignPermissionsRequest request);
    }
}
