using GymManagementSystem.Business.Abstract;
using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.Entities.Concrete;

namespace GymManagementSystem.Business.Concrete;

public class ServiceManager:GenericManager<Service>,IServiceService
{
    private readonly IServiceRepository _serviceRepository;

    public ServiceManager(IServiceRepository serviceRepository) : base(serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }
}
