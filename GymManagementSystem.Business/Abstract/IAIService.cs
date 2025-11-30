namespace GymManagementSystem.Business.Abstract;

public interface IAIService
{
    Task<string> GetGymWorkoutPlanAsync(string prompt);
    Task<string> CheckAvailableModelsAsync();
}
