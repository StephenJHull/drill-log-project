using Microsoft.AspNetCore.Identity;
using System.IO.Abstractions;
using System.Text.Json;
using WebApp.Models;

namespace WebApp.Initilization
{
    public class IdentityInitializer
    {
        private readonly IFileSystem _fileSystem;
        private readonly IServiceProvider _serviceProvider;

        public IdentityInitializer(IFileSystem fileSystem, IServiceProvider serviceProvider)
        {
            _fileSystem = fileSystem;
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeAsync()
        {
            var rootPath = _fileSystem.Directory.GetCurrentDirectory();

            var filePath = _fileSystem.Path.Combine(rootPath, "Users.json");
            if (!_fileSystem.File.Exists(filePath))
            {
                return;
            }

            var json = _fileSystem.File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            var identityUsers = JsonSerializer.Deserialize<IEnumerable<UserModel>>(json);
            if (identityUsers == null || !identityUsers.Any())
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = identityUsers.SelectMany(x => x.Roles).Distinct();

            await AddRolesAsync(roles, roleManager);

            foreach (var identityUser in identityUsers)
            {
                await AddUserAndRolesAsync(identityUser, userManager, roleManager);
            }

            _fileSystem.File.Delete(filePath);
        }

        private async Task AddRolesAsync(IEnumerable<string> roles, RoleManager<IdentityRole> roleManager)
        {
            var existingRoles = roleManager.Roles.Select(x => x.Name).ToList();

            var rolesToAdd = roles.Where(x => !existingRoles.Contains(x)).ToList();

            foreach (var role in rolesToAdd)
            {
                var result = await roleManager.CreateAsync(new IdentityRole { Name = role });
                if (!result.Succeeded)
                {
                    throw new Exception($"The {role} role was not created.");
                }
            }
        }

        private async Task AddUserAndRolesAsync(UserModel identityUser, UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            var users = userManager.Users.ToList();

            var email = identityUser.Email;
            var user = users.FirstOrDefault(x => x.UserName == email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(user, identityUser.Password);
                if (!result.Succeeded)
                {
                    throw new Exception($"The {email} user was not created.");
                }
            }

            var roles = await userManager.GetRolesAsync(user);

            if (!roles.Any())
            {
                var rolesToAdd = identityUser.Roles;
                foreach (var roleToAdd in rolesToAdd)
                {
                    var result = await userManager.AddToRoleAsync(user, roleToAdd);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"The {email} user was not added to the {roleToAdd} role.");
                    }
                }
            }
        }
    }
}
