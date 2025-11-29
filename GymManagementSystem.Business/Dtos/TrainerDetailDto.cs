namespace GymManagementSystem.Business.Dtos;

public class TrainerDetailDto
{
    public TrainerDetailDto()
    {
        ServiceList = new List<ServiceCheckBoxDto>();
    }
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public int? GymId { get; set; }
    public TimeSpan? ShiftStart { get; set; }
    public TimeSpan? ShiftEnd { get; set; }

    public List<ServiceCheckBoxDto> ServiceList { get; set; }
}

public class ServiceCheckBoxDto
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; }
    public bool IsSelected { get; set; }
}
