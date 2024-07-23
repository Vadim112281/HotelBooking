using BookingHotel.Constants;
using BookingHotel.Data;
using BookingHotel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BookingHotel.Services
{
    public interface IUserService
    {
        Task<MethodResult<ApplicationUser>> CreateUserAsync(ApplicationUser user, string email, string password);
        Task<PageResult<UserDisplayModel>> GetUsersAsync(int startIndex,int pageSize, RoleType? roleType = null);
        Task<MyProfileModel?> GetProfileDetailsAsync(string userId);
        Task<string> UpdateProfileAsync(MyProfileModel model, RoleType? roleType = null);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;

        public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
        }

        public async Task<PageResult<UserDisplayModel>> GetUsersAsync(int startIndex, int pageSize, RoleType? roleType = null)
        {
            var query = _userManager.Users;
            if(roleType is not null)
            {
                query = query.Where(x => x.RoleName == roleType.ToString());
            }

            var total = await query.CountAsync();

            var records =  await query.Select(x => new UserDisplayModel(x.Id, x.FullName, x.Email, x.RoleName, x.ContactNumber, x.Designation))
                                .Skip(startIndex)
                                .Take(pageSize)
                                .ToListAsync();

            return new PageResult<UserDisplayModel>(total, records);
        }

        public async Task<MethodResult<ApplicationUser>> CreateUserAsync(ApplicationUser user, string email, string password)
        {
            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            var emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return $"Error: {string.Join(", ", result.Errors.Select(error => error.Description))}";
            }

            result = await _userManager.AddToRoleAsync(user, user.RoleName ?? RoleType.Guest.ToString());

            if (!result.Succeeded)
            {
                return $"Error: {string.Join(", ", result.Errors.Select(error => error.Description))}";
            }
            return user;
        }

        public async Task<MyProfileModel?> GetProfileDetailsAsync(string userId)
        {
            var staff =  await GetUser(userId)
                               .Select(x => new MyProfileModel
                               {
                                    Id = x.Id,
                                    ContactNumber = x.ContactNumber,
                                    Designation = x.Designation, 
                                    Email = x.Email,
                                    FirstName = x.FirstName,
                                   LastName = x.LastName
                                })
                               .FirstOrDefaultAsync();

            return staff;
        }

        private IQueryable<ApplicationUser> GetUser(string userId, RoleType? roleType = null)
        {
            var query = _userManager.Users.Where(x => x.Id == userId);
            if(roleType is not null)
            {
                query = query.Where(x => x.RoleName == RoleType.Staff.ToString());
            }
            return query;
        }    

        public async Task<string> UpdateProfileAsync(MyProfileModel model, RoleType? roleType = null)
        {
            var dbUser = await GetUser(model.Id, roleType).FirstOrDefaultAsync();
            if(dbUser is null)
            {
                return "Invalid request";
            }

            dbUser.FirstName = model.FirstName;
            dbUser.LastName = model.LastName;
            dbUser.Designation = model.Designation;
            dbUser.ContactNumber = model.ContactNumber;

            var result = await _userManager.UpdateAsync(dbUser);
            if (!result.Succeeded)
            {
                return $"Error: {string.Join(", ", result.Errors.Select(error => error.Description))}";
            }

            if(dbUser.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
            {
                // Admin didn`t change the email id 
                return "true";
            }

            // Admin did change Staff Email Id

            var changeToken = await _userManager.GenerateChangeEmailTokenAsync(dbUser, model.Email);

            result = await _userManager.ChangeEmailAsync(dbUser, model.Email, changeToken);
            if (!result.Succeeded)
            {
                return $"Error: {string.Join(", ", result.Errors.Select(error => error.Description))}";
            }
            return "true";
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
