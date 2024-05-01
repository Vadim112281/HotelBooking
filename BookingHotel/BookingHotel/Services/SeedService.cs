using BookingHotel.Data;
using Microsoft.AspNetCore.Identity;

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

        }
    }
}
