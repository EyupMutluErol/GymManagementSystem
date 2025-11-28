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

    public async Task ChangeUserRoleAsync(int userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user != null)
        {
            // 1. Mevcut tüm rolleri al
            var currentRoles = await _userManager.GetRolesAsync(user);

            // 2. Mevcut rolleri sil (Temizlik)
            if (currentRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // 3. Yeni rolü ekle
            await _userManager.AddToRoleAsync(user, newRole);
        }
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
    }
}
