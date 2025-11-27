using GymManagementSystem.Business.Dtos;
using GymManagementSystem.Entities.Concrete;


namespace GymManagementSystem.Business.Abstract;

public interface IAppUserService : IGenericService<AppUser>
{
    Task<List<AppUser>> GetUsersByRoleAsync(string roleName);
    Task<List<UserListDto>> GetUserListWithRolesAsync();
}
