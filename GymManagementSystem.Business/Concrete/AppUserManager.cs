using GymManagementSystem.Business.Abstract;
using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.Business.Concrete;

public class AppUserManager:GenericManager<AppUser>,IAppUserService
{
    private readonly IAppUserRepository _appUserRepository;

    public AppUserManager(IAppUserRepository appUserRepository) : base(appUserRepository)
    {
        _appUserRepository = appUserRepository;
    }
}
