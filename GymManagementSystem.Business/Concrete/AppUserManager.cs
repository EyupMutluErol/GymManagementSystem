using GymManagementSystem.Business.Abstract;
using GymManagementSystem.Business.Dtos;
using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.DataAccess.Concrete.EntityFramework;
using GymManagementSystem.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Business.Concrete;

public class AppUserManager:GenericManager<AppUser>,IAppUserService
{
    private readonly IAppUserRepository _appUserRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IServiceRepository _serviceRepository;
    private readonly ITrainerServiceRepository _trainerServiceRepository;
    private readonly IGymRepository _gymRepository;

    public AppUserManager(IAppUserRepository appUserRepository, UserManager<AppUser> userManager, IServiceRepository serviceRepository, ITrainerServiceRepository trainerServiceRepository, IGymRepository gymRepository) : base(appUserRepository)
    {
        _appUserRepository = appUserRepository;
        _userManager = userManager;
        _serviceRepository = serviceRepository;
        _trainerServiceRepository = trainerServiceRepository;
        _gymRepository = gymRepository;
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

            if (newRole == "Member")
            {
                user.GymId = null;       // Salon bağını kopar
                user.ShiftStart = null;  // Vardiyayı temizle
                user.ShiftEnd = null;

                // Kullanıcıyı güncelle
                await _userManager.UpdateAsync(user);

                var services = _trainerServiceRepository.GetListByFilter(x => x.AppUserId == user.Id);
                foreach (var item in services)
                {
                    _trainerServiceRepository.Delete(item);
                }
            }
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

    public async Task<TrainerDetailDto> GetTrainerDetailsAsync(int trainerId)
    {
        var user = await _userManager.Users
            .Include(u => u.TrainerServices) // Mevcut uzmanlıklarını da çek
            .FirstOrDefaultAsync(u => u.Id == trainerId);

        if (user == null) return null;

        var dto = new TrainerDetailDto
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            GymId = user.GymId,
            ShiftStart = user.ShiftStart,
            ShiftEnd = user.ShiftEnd,
            ServiceList = new List<ServiceCheckBoxDto>()
        };

        // Eğer bir salona atanmışsa, o salonun hizmetlerini listele
        if (user.GymId.HasValue)
        {
            // Salondaki tüm hizmetleri çek
            var gymServices = _serviceRepository.GetListByFilter(x => x.GymId == user.GymId.Value);

            foreach (var service in gymServices)
            {
                // Hoca bu hizmeti veriyor mu?
                bool isAssigned = user.TrainerServices.Any(ts => ts.ServiceId == service.Id);

                dto.ServiceList.Add(new ServiceCheckBoxDto
                {
                    ServiceId = service.Id,
                    ServiceName = service.Name,
                    IsSelected = isAssigned
                });
            }
        }

        return dto;
    }

    public async Task UpdateTrainerDetailsAsync(TrainerDetailDto trainerDto)
    {
        var user = await _userManager.FindByIdAsync(trainerDto.UserId.ToString());

        // Salonu bul (Saatlerini kontrol etmek için)
        var gym = trainerDto.GymId.HasValue ? _gymRepository.GetById(trainerDto.GymId.Value) : null;

        // --- SAAT KONTROLÜ BAŞLANGIÇ ---
        if (user != null && gym != null && trainerDto.ShiftStart.HasValue && trainerDto.ShiftEnd.HasValue)
        {
            // 1. Mantık Hatası: Başlangıç saati, bitiş saatinden büyük olamaz
            if (trainerDto.ShiftStart >= trainerDto.ShiftEnd)
            {
                throw new System.Exception("Mesai başlangıç saati, bitiş saatinden önce olmalıdır.");
            }

            // 2. Salon Saati Kontrolü
            // Hocanın başladığı saat, salonun açılışından ÖNCE olamaz.
            // Hocanın bittiği saat, salonun kapanışından SONRA olamaz.
            if (trainerDto.ShiftStart < gym.OpenTime || trainerDto.ShiftEnd > gym.CloseTime)
            {
                throw new System.Exception($"Hata: Antrenörün çalışma saatleri ({trainerDto.ShiftStart}-{trainerDto.ShiftEnd}), salonun çalışma saatleri ({gym.OpenTime}-{gym.CloseTime}) sınırları içinde olmalıdır.");
            }
        }
        // --- SAAT KONTROLÜ BİTİŞ ---

        if (user != null)
        {
            // Kontrolleri geçtiyse veriyi güncelle
            user.GymId = trainerDto.GymId;
            user.ShiftStart = trainerDto.ShiftStart;
            user.ShiftEnd = trainerDto.ShiftEnd;

            await _userManager.UpdateAsync(user);

            // Hizmetleri güncelle
            var oldServices = _trainerServiceRepository.GetListByFilter(x => x.AppUserId == user.Id);
            foreach (var old in oldServices)
            {
                _trainerServiceRepository.Delete(old);
            }

            if (trainerDto.ServiceList != null)
            {
                foreach (var item in trainerDto.ServiceList)
                {
                    if (item.IsSelected)
                    {
                        _trainerServiceRepository.Insert(new TrainerService
                        {
                            AppUserId = user.Id,
                            ServiceId = item.ServiceId
                        });
                    }
                }
            }
        }
    }
}
