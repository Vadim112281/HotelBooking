using BookingHotel.Constants;
using BookingHotel.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BookingHotel.Services
{
    public class SeedService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleStore<IdentityRole> _roleStore;
        private readonly IConfiguration _configuration;
        public SeedService(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, 
            RoleManager<IdentityRole> roleManager, IRoleStore<IdentityRole> roleStore, IConfiguration configuration)
        {
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;
            _roleStore = roleStore;
            _configuration = configuration;
        }

        public async Task SeedDatabaseAsync()
        {
            var adminUserEmail = _configuration.GetValue<string>("AdminUser: Email");

            var dbAdminUser = _userManager.FindByEmailAsync(adminUserEmail);
            if(dbAdminUser is not null)
            {
                return; //Database already has Admin User. No need to do anything
            }

            var applicationUser = new ApplicationUser()
            {
                FirstName = _configuration.GetValue<string>("AdminUser: FirstName")!,
                LastName = _configuration.GetValue<string>("AdminUser: LastName"),
                RoleName = RoleType.Admin.ToString(),
                ContactNumber = _configuration.GetValue<string>("AdminUser: ContactNumber")!,
                Designation = "Administrator"
            };

            await _userStore.SetUserNameAsync(applicationUser, adminUserEmail, default);
            var emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
            await emailStore.SetEmailAsync(applicationUser, adminUserEmail, default);

            var result = await _userManager.CreateAsync(applicationUser, _configuration.GetValue<string>("AdminUser: Password")!);

            if (!result.Succeeded)
            {
                var errors = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
                throw new Exception(errors);
            }

            if(await _roleManager.FindByNameAsync(RoleType.Admin.ToString()) is null)
            {
                foreach(var roleName in Enum.GetNames<RoleType>())
                {
                    var role = new IdentityRole
                    {
                        Name = roleName
                    };
                    await _roleManager.CreateAsync(role);
                }
            }
        }
    }
}
