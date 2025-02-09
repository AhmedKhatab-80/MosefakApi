namespace MosefakApp.Infrastructure.Identity.Seeding
{
    public static class IdentitySeeding
    {
        public static async Task<IServiceCollection> Seed(this IServiceCollection services, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, AppIdentityDbContext context)
        {
            if (!userManager.Users.Any())
            {
                var hasher = new PasswordHasher<AppUser>();

                foreach (var user in LoadUsers())
                {
                    var passwordHashed = hasher.HashPassword(user, DefaultUser.AdminPassword);

                    await userManager.CreateAsync(user, passwordHashed);
                }
            }

            if (!roleManager.Roles.Any())
            {
                foreach (var role in LoadRoles())
                {
                    await roleManager.CreateAsync(role);
                }
            }

            if (!context.UserRoles.Any())
            {
                foreach (var userRole in LoadUserRoles())
                {
                    await context.UserRoles.AddAsync(userRole);
                }

                await context.SaveChangesAsync();
            }


            if (!context.RoleClaims.Any())
            {
                foreach (var RoleClaim in LoadPermissions())
                {
                    await context.RoleClaims.AddAsync(RoleClaim);
                }

                await context.SaveChangesAsync();
            }

            return services;
        }

        private static IEnumerable<AppUser> LoadUsers()
        {
            return new List<AppUser>
            {
                new AppUser()
                {
                    FirstName = DefaultUser.FirstNameAdmin,
                    LastName = DefaultUser.LastNameAdmin,
                    UserName = DefaultUser.AdminEmail.Split('@')[0],
                    Email = DefaultUser.AdminEmail,
                    EmailConfirmed = true, // beacuse this is admin
                    Address = new Address()
                    {
                        //AppUserId = 2,
                        State = DefaultUser.State,
                        City = DefaultUser.City,
                        Street = DefaultUser.Street,
                        ZipCode = DefaultUser.ZipCode
                    }
                }
            };
        }

        private static IEnumerable<AppRole> LoadRoles()
        {
            return new List<AppRole>()
            {
                new AppRole
                {
                    Name = DefaultRole.Admin,
                    ConcurrencyStamp = DefaultRole.AdminConcurrencyStamp,
                    CreationTime = DateTime.UtcNow,
                },
                new AppRole
                {
                    Name = DefaultRole.Doctor,
                    ConcurrencyStamp = DefaultRole.DoctorConcurrencyStamp,
                    CreationTime = DateTime.UtcNow,
                },
                new AppRole
                {
                    Name = DefaultRole.Patient,
                    ConcurrencyStamp = DefaultRole.PatientConcurrencyStamp,
                    CreationTime = DateTime.UtcNow,
                }
            };
        }

        private static IEnumerable<IdentityUserRole<int>> LoadUserRoles()
        {
            return new List<IdentityUserRole<int>>()
            {
                new IdentityUserRole<int>
                {
                    RoleId = DefaultRole.AdminId,
                    UserId = DefaultUser.AdminId,
                }
            };
        }

        private static IEnumerable<IdentityRoleClaim<int>> LoadPermissions()
        {
            var list = new List<IdentityRoleClaim<int>>();

            foreach (var permisssion in Permissions.GetPermissions())
            {
                list.Add(new IdentityRoleClaim<int>
                {
                    ClaimType = Permissions.Type,
                    RoleId = DefaultRole.AdminId,
                    ClaimValue = permisssion
                });
            }

            return list;
        }
    }
}
