using ActivitySystem.Domain.Entities;
using ActivitySystem.Infrastructure.Database;
using ActivitySystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class ActivityRepository : IActivityRepository
{
    private readonly ApplicationDbContext _context;
    public ActivityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Activity activity)
    {
        await _context.Activities.AddAsync(activity);
    }

    public async Task UpdateAsync(Activity activity)
    {
        _context.Activities.Update(activity);
    }

    public async Task DeleteAsync(int id)
    {
        var activity = await _context.Activities.FindAsync(id);
        if (activity != null)
        {
            activity.IsDeleted = true;
        }
    }

    public async Task<Activity> GetByIdAsync(int id)
    {
        return await _context.Activities
            .Include(a => a.Organizer)
            .Include(a => a.Subscriptions)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Activity>> GetAllActivitiesAsync()
    {
        return await _context.Activities
            .Include(a => a.Organizer)
            .Where(a => !a.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Activity>> GetActivitiesByOrganizerAsync(string organizerId)
    {
        return await _context.Activities
            .Include(a => a.Organizer)
            .Where(a => a.OrganizerId == organizerId && !a.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}