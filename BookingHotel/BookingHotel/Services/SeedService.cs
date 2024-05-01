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
        public SeedService(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, 
            RoleManager<IdentityRole> roleManager, IRoleStore<IdentityRole> roleStore)
        {
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;
            _roleStore = roleStore;
        }
    }
}
