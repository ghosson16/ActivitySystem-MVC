using ActivitySystem.Domain.Entities;

namespace ActivitySystem.Infrastructure.Repositories
{
    public interface IActivityRepository
    {
        Task<IEnumerable<Activity>> GetActivitiesByOrganizerAsync(string organizerId);
        Task<IEnumerable<Activity>> GetAllActivitiesAsync();
        Task AddAsync(Activity activity);
        Task UpdateAsync(Activity activity);
        Task DeleteAsync(int id);
        Task<Activity> GetByIdAsync(int id);
        Task<bool> SaveAsync();
    }
}
