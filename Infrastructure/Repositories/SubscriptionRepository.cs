using ActivitySystem.Domain.Entities;
using ActivitySystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ActivitySystem.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;
        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
        }

        public async Task DeleteAsync(string guestId, int activityId)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.ActivityId == activityId
                && s.GuestId == guestId);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
            }
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsByActivityAsync(int activityId)
        {
            return await _context.Subscriptions
                .Include(s => s.Guest)
                .Where(s => s.ActivityId == activityId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsByGuestAsync(string guestId)
        {
            return await _context.Subscriptions
                .Include(s => s.Activity)
                .Where(s => s.GuestId == guestId)
                .ToListAsync();
        }

        public async Task<int> GetTotalParticipantsByOrganizerAsync(string organizerId)
        {
            return await _context.Subscriptions
                .CountAsync(s => s.Activity.OrganizerId == organizerId && !s.Activity.IsDeleted);
        }

        public async Task<bool> IsSubscribedAsync(string guestId, int activityId)
        {
            return await _context.Subscriptions
                .AnyAsync(s => s.ActivityId == activityId
                && s.GuestId == guestId);
        }

        public async Task MarkAsAttendedAsync(int activityId, string guestId)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.ActivityId == activityId && s.GuestId == guestId);

            if (subscription != null)
            {
                subscription.IsAttended = true;
            }
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
