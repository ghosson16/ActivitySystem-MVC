using ActivitySystem.Domain.Entities;
using ActivitySystem.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ActivitySystem.Infrastructure
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roles = { "Organizer", "Guest" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. إنشاء المنظمين مع الأرقام والوصف
            var organizers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "sami.org@events.com",
                    Email = "sami.org@events.com",
                    Name = "Sami Mansour",
                    PhoneNumber = "+966501112233",
                    Description = "Tech Academy: A leading center focused on empowering developers through hands-on workshops and coding bootcamps in Riyadh.",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    UserName = "nora.org@events.com",
                    Email = "nora.org@events.com",
                    Name = "Nora Ali",
                    PhoneNumber = "+966554445566",
                    Description = "Innovate Events: A specialized agency dedicated to organizing high-impact tech conferences and professional networking gatherings.",
                    EmailConfirmed = true
                }
            };

            foreach (var org in organizers)
            {
                var existingUser = await userManager.FindByEmailAsync(org.Email);
                if (existingUser == null)
                {
                    await userManager.CreateAsync(org, "Org@123");
                    await userManager.AddToRoleAsync(org, "Organizer");
                }
            }

            if (!context.Activities.Any())
            {
                var sami = await userManager.FindByEmailAsync("sami.org@events.com");
                var nora = await userManager.FindByEmailAsync("nora.org@events.com");

                context.Activities.AddRange(new List<Activity>
                {
                    new Activity
                    {
                        Title = "Full-Stack BootCamp",
                        Type = "Workshop",
                        Capacity = 30,
                        Location = "Riyadh",
                        Date = DateTime.Now.AddDays(10),
                        OrganizerId = sami.Id,
                        Description = "Hands-on coding workshop.",
                        ImageUrl = "62b6116e-1f38-448e-bc2a-7c690a98906f.jpeg"
                    },
                    new Activity
                    {
                        Title = "Tech Networking Night",
                        Type = "Social",
                        Capacity = 50,
                        Location = "Sky Tower",
                        Date = DateTime.Now.AddDays(15),
                        OrganizerId = sami.Id,
                        Description = "Meet professional developers.",
                        ImageUrl = "d352ed32-b7d7-461d-ae8c-c6c97810c996.jpeg"
                    },
                    new Activity
                    {
                        Title = "Global AI Summit",
                        Type = "Conference",
                        Capacity = 200,
                        Location = "Convention Center",
                        Date = DateTime.Now.AddDays(25),
                        OrganizerId = nora.Id,
                        Description = "Exploring future tech.",
                        ImageUrl = "e5f52f2c-1c54-4039-926a-a665cee923ae.jpeg"
                    },
                    new Activity
                    {
                        Title = "Developer Soccer Cup",
                        Type = "Sports",
                        Capacity = 22,
                        Location = "City Stadium",
                        Date = DateTime.Now.AddDays(5),
                        OrganizerId = nora.Id,
                        Description = "A friendly sports event.",
                        ImageUrl = "fe8fead9-89b5-4a19-87ba-ff5f856f350a.jpeg"
                    }
                });
                await context.SaveChangesAsync();
            }

            var guestNames = new List<string> { "Liam", "Noah", "Oliver", "James", "Emma", "Sophia", "Mia", "Isabella" };
            int phoneSuffix = 1;
            foreach (var name in guestNames)
            {
                var email = $"{name.ToLower()}@guest.com";
                var existingGuest = await userManager.FindByEmailAsync(email);
                if (existingGuest == null)
                {
                    var guest = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        Name = $"{name} Smith",
                        Age = 25,
                        PhoneNumber = $"+96650000000{phoneSuffix++}",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(guest, "Guest@123");
                    await userManager.AddToRoleAsync(guest, "Guest");
                }
            }

            if (!context.Subscriptions.Any())
            {
                var allActivities = context.Activities.ToList();
                var allGuests = await userManager.GetUsersInRoleAsync("Guest");

                foreach (var guest in allGuests)
                {
                    context.Subscriptions.Add(new Subscription { ActivityId = allActivities[0].Id, GuestId = guest.Id, IsAttended = true });
                    context.Subscriptions.Add(new Subscription { ActivityId = allActivities[1].Id, GuestId = guest.Id, IsAttended = false });
                }
                await context.SaveChangesAsync();
            }
        }
    }
}