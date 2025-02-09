namespace MosefakApi.Business.Services.Role
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly AppIdentityDbContext _Context;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        public RoleService(RoleManager<AppRole> roleManager, ICacheService cacheService, IMapper mapper, AppIdentityDbContext context)
        {
            _roleManager = roleManager;
            _cacheService = cacheService;
            _mapper = mapper;
            _Context = context;
        }

        #region get roles without claims that has it
        public async Task<IList<RoleResponse>> GetRolesAsync(bool IncludeDeleted = false)
        {
            var rolesQuery = _roleManager.Roles.AsQueryable();

            if (!IncludeDeleted)
            {
                rolesQuery = rolesQuery.Where(x => !x.IsDeleted);
            }

            var rolesResponse = await rolesQuery.Select(x => new RoleResponse
            {
                Id = x.Id.ToString(),
                Name = x.Name!,
                IsDeleted = x.IsDeleted,
            })
                .ToListAsync();

            if (!rolesResponse.Any())
            {
                throw new ItemNotFound("No roles exist.");
            }

            return rolesResponse;
        }
        #endregion

        #region get roles with claims that has it
        public async Task<IList<RoleResponse>> GetRolesAsync2(bool IncludeDeleted = false)
        {
            var rolesQuery = _roleManager.Roles.AsQueryable();

            if (!IncludeDeleted)
            {
                rolesQuery = rolesQuery.Where(x => !x.IsDeleted);
            }

            var roles = await rolesQuery.ToListAsync();

            var rolesResponse = (from r in roles
                                 join c in _Context.RoleClaims
                                 on r.Id equals c.RoleId into claims
                                 select new RoleResponse
                                 {
                                     Id = r.Id.ToString(),
                                     Name = r.Name!,
                                     IsDeleted = r.IsDeleted,
                                     Permissions = claims.Select(x => x.ClaimValue).ToList()!
                                 })
                                 .ToList();


            if (!rolesResponse.Any())
            {
                throw new ItemNotFound("No roles exist.");
            }

            return rolesResponse;
        }
        #endregion

        #region get roles with claims that has it by another implementaion but not the best as performance beacuse talk database so many to execute query
        public async Task<IList<RoleResponse>> GetRolesAsync3(bool IncludeDeleted = false)
        {
            var rolesQuery = _roleManager.Roles.AsQueryable();

            if (!IncludeDeleted)
            {
                rolesQuery = rolesQuery.Where(x => !x.IsDeleted);
            }

            var roles = await rolesQuery.ToListAsync();

            if (!roles.Any())
            {
                throw new ItemNotFound("No roles exist.");
            }

            var list = new List<RoleResponse>();

            foreach (var role in roles)
            {
                var claims = await _roleManager.GetClaimsAsync(role);

                list.Add(new RoleResponse
                {
                    Id = role.Id.ToString(),
                    Name = role.Name!,
                    IsDeleted = role.IsDeleted,
                    Permissions = claims.Select(x => x.Value).ToList()! ?? new List<string>()
                });
            }

            return list;
        }
        #endregion

        #region Get Role without Claims that has it...
        public async Task<RoleResponse> GetRoleByIdAsync(int id)
        {
            var roleResponse = await _roleManager.Roles.Select(x => new RoleResponse
            {
                Id = x.Id.ToString(),
                Name = x.Name!,
                IsDeleted = x.IsDeleted,
            })
               .FirstOrDefaultAsync(x => x.Id == id.ToString());


            if (roleResponse is null)
                throw new ItemNotFound("Not Exist Role with this id");

            return roleResponse;
        }
        #endregion

        #region Get Role with Claims that has it by another implementation...
        public async Task<RoleResponse> GetRoleByIdAsync3(int id)
        {
            var roleResponse = await (from r in _roleManager.Roles
                                      where r.Id == id
                                      join c in _Context.RoleClaims
                                      on r.Id equals c.RoleId into claims
                                      select new RoleResponse
                                      {
                                          Id = r.Id.ToString(),
                                          Name = r.Name!,
                                          IsDeleted = r.IsDeleted,
                                          Permissions = claims.Select(x => x.ClaimValue).ToList()!
                                      })
                                         .FirstOrDefaultAsync();

            if (roleResponse is null)
                throw new ItemNotFound("Not Exist Role with this id");

            return roleResponse;
        }
        #endregion

        #region Get Role with Claims that has it...
        public async Task<RoleResponse> GetRoleByIdAsync2(int id)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);

            if (role is null)
                throw new ItemNotFound("Not Exist Role with this id");

            var claims = await _roleManager.GetClaimsAsync(role);

            var roleResponse = new RoleResponse
            {
                Id = role.Id.ToString(),
                Name = role.Name!,
                IsDeleted = role.IsDeleted,
                Permissions = claims.Select(x => x.Value).ToList()! ?? new List<string>(),
            };

            return roleResponse;
        }
        #endregion

        #region Create Role with Claims(permissions) that has it..
        public async Task<RoleResponse> AddRoleAsync(RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new BadRequest("Role name cannot be null or empty");

            var IsExist = await _roleManager.RoleExistsAsync(request.Name);

            if (IsExist)
                throw new ItemAlreadyExist($"{request.Name} role already exist");

            var allowedClaims = await _Context.RoleClaims.Select(x => x.ClaimValue).ToListAsync();

            if (request.Permissions.Except(allowedClaims).Any())
            {
                throw new BadRequest("Invalid permissions sended");
            }

            var role = _mapper.Map<AppRole>(request);

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();

                throw new InvalidOperation(error?.Description ?? "An unknown error occurred while creating the role.");
            }

            if (request.Permissions is not null && request.Permissions.Any())
            {
                foreach (var permission in request.Permissions)
                {
                    var identityResult = await _roleManager.AddClaimAsync(role, new Claim(Permissions.Type, permission));

                    if (!identityResult.Succeeded)
                    {
                        var error = identityResult.Errors.Select(x => x.Description).FirstOrDefault() ?? "Can't assign To Roles";

                        throw new BadRequest(error);
                    }
                }
            }

            await _cacheService.RemoveCachedResponseAsync($"/api/Roles");

            return GetRoleResponse(role, request);
        }
        #endregion

        #region Edit Role
        public async Task<RoleResponse> EditRoleAsync(int id, RoleRequest request)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);

            if (role == null)
                throw new ItemNotFound("Role is not found");

            if (string.IsNullOrEmpty(request.Name))
                throw new BadRequest("Role name cannot be null or empty");

            var allowedClaims = await _Context.RoleClaims.Select(x => x.ClaimValue).ToListAsync();

            if (request.Permissions.Except(allowedClaims).Any())
            {
                throw new BadRequest("Invalid permissions sended");
            }

            role.Name = request.Name;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();

                throw new InvalidOperation(error?.Description ?? "An unknown error occurred while creating the role.");
            }

            if (request.Permissions is not null && request.Permissions.Any())
            {
                await _Context.RoleClaims.Where(x => x.RoleId == id).ExecuteDeleteAsync();

                foreach (var claim in request.Permissions)
                {
                    var identityResult = await _roleManager.AddClaimAsync(role, new Claim(Permissions.Type, claim));

                    if (!identityResult.Succeeded)
                    {
                        var error = identityResult.Errors.Select(x => x.Description).FirstOrDefault() ?? "Can't assign To Roles";

                        throw new BadRequest(error);
                    }
                }
            }

            await _cacheService.RemoveCachedResponseAsync($"/api/Roles");
            await _cacheService.RemoveCachedResponseAsync($"/api/Roles/{id}");

            return GetRoleResponse(role, request);
        }
        #endregion

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);

            if (role == null)
                throw new ItemNotFound("Role is not found");

            role.IsDeleted = !role.IsDeleted;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();

                throw new InvalidOperation(error?.Description ?? "An unknown error occurred while creating the role.");
            }

            await _cacheService.RemoveCachedResponseAsync($"/api/Roles/{id}");
            await _cacheService.RemoveCachedResponseAsync($"/api/Roles|IncludeDeleted-false");
        }

        public async Task<bool> AssignPermissionToRoleAsync(string roleId, AssignPermissionsRequest request)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                throw new ItemNotFound("Role not found");

            var claims = request.Permissions.Select(p => new Claim(Permissions.Type, p)).ToList();

            foreach (var claim in claims)
            {
                if (!(await _roleManager.GetClaimsAsync(role)).Any(c => c.Type == Permissions.Type && c.Value == claim.Value))
                {
                    var result = await _roleManager.AddClaimAsync(role, claim);
                    if (!result.Succeeded)
                        throw new BadRequest("Failed to assign permissions");
                }
            }

            await _cacheService.RemoveCachedResponseAsync($"/api/Roles/{roleId}");
            await _cacheService.RemoveCachedResponseAsync($"/api/Roles|IncludeDeleted-false");

            return true;
        }

        private RoleResponse GetRoleResponse(AppRole role, RoleRequest request)
        {
            return new RoleResponse
            {
                Id = role.Id.ToString(),
                Name = role.Name!,
                IsDeleted = role.IsDeleted,
                Permissions = request.Permissions ?? new List<string>()
            };
        }
    }
}
