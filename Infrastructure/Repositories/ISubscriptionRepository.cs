using ActivitySystem.Domain.Entities;

namespace ActivitySystem.Infrastructure.Repositories
{
    public interface ISubscriptionRepository
    {
        Task AddAsync(Subscription subscription);
        Task DeleteAsync(string guestId, int activityId);
        Task<IEnumerable<Subscription>> GetSubscriptionsByGuestAsync(string guestId);
        Task<IEnumerable<Subscription>> GetSubscriptionsByActivityAsync(int activityId);
        Task<int> GetTotalParticipantsByOrganizerAsync(string organizerId);
        Task<bool> IsSubscribedAsync(string guestId, int activityId);
        Task MarkAsAttendedAsync(int activityId, string guestId);
        Task<bool> SaveAsync();
    }
}
