using GymManagementSystem.Business.Dtos;
using GymManagementSystem.Entities.Concrete;


namespace GymManagementSystem.Business.Abstract;

public interface IAppUserService : IGenericService<AppUser>
{
    Task<List<AppUser>> GetUsersByRoleAsync(string roleName);
    Task<List<UserListDto>> GetUserListWithRolesAsync();
    Task ChangeUserRoleAsync(int userId, string newRole);
    Task DeleteUserAsync(int userId);
    Task<TrainerDetailDto> GetTrainerDetailsAsync(int trainerId);
    Task UpdateTrainerDetailsAsync(TrainerDetailDto trainerDto);
}
