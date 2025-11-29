namespace GymManagementSystem.WebUI.Models.ViewModels;

public class AppointmentListViewModel
{
    public int Id { get; set; }
    public string GymName { get; set; }
    public string ServiceName { get; set; }
    public string TrainerName { get; set; }
    public string MemberName { get; set; }
    public DateTime Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Status { get; set; } 
    public string StatusColor { get; set; }
}
