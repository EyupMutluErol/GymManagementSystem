using GymManagementSystem.Business.Abstract;
using GymManagementSystem.Business.Dtos;
using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.Entities.Concrete;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Business.Concrete;

public class AppUserManager:GenericManager<AppUser>,IAppUserService
{
    private readonly IAppUserRepository _appUserRepository;
    private readonly UserManager<AppUser> _userManager;

    public AppUserManager(IAppUserRepository appUserRepository, UserManager<AppUser> userManager) : base(appUserRepository)
    {
        _appUserRepository = appUserRepository;
        _userManager = userManager;
    }

    public async Task<List<AppUser>> GetUsersByRoleAsync(string roleName)
    {
        var users = await _userManager.GetUsersInRoleAsync(roleName);
        return users.ToList();
    }

    public async Task<List<UserListDto>> GetUserListWithRolesAsync()
    {
        var users = _userManager.Users.ToList();
        var dtoList = new List<UserListDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "Rol Yok";

            if (userRole != "Admin")
            {
                dtoList.Add(new UserListDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RoleName = userRole
                });
            }
        }
        return dtoList;
    }
}
