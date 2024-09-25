using Bogus;
using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Data
{
    public static class SeedData
    {
        public static async Task SeedDataAsync(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var servicesProvider = scope.ServiceProvider;
                var db = servicesProvider.GetRequiredService<LMSApiContext>();
                var userManager = servicesProvider.GetRequiredService<UserManager<User>>();
                var roleManager = servicesProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await db.Database.MigrateAsync();

                if (await db.Courses.AnyAsync()) return; // If data exists, don't seed again

                try
                {
                    // Create Roles
                    await CreateRolesAsync(roleManager);

                    // Generate Courses
                    var courses = GenerateCourses(5);
                    await db.AddRangeAsync(courses);
                    await db.SaveChangesAsync();

                    // Generate Modules for the Courses
                    var modules = GenerateModules(10, courses);
                    await db.AddRangeAsync(modules);
                    await db.SaveChangesAsync();

                    // Generate Activity Types
                    var activityTypes = GenerateActivityTypes(5);
                    await db.AddRangeAsync(activityTypes);
                    await db.SaveChangesAsync();

                    // Generate Activities for the Modules and Activity Types
                    var activities = GenerateActivities(15, modules, activityTypes);
                    await db.AddRangeAsync(activities);
                    await db.SaveChangesAsync();

                    // Generate Users and assign to Courses
                    await GenerateUsersAsync(userManager, 10, courses);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message); // Log error for debugging
                    throw;
                }
            }
        }

        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Teacher", "Student" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static List<Course> GenerateCourses(int count)
        {
            var faker = new Faker<Course>()
                .RuleFor(c => c.Name, f => f.Lorem.Word())
                .RuleFor(c => c.Description, f => f.Lorem.Sentence())
                .RuleFor(c => c.StartDate, f => f.Date.Past());

            return faker.Generate(count);
        }

        private static List<Module> GenerateModules(int count, List<Course> courses)
        {
            var modules = new List<Module>();
            var faker = new Faker<Module>()
                .RuleFor(m => m.Name, f => f.Lorem.Word())
                .RuleFor(m => m.Description, f => f.Lorem.Sentence());

            Random random = new Random();

            foreach (var course in courses)
            {
                DateTime currentStartDate = course.StartDate;

                for (int i = 0; i < count; i++)
                {
                    
                    int moduleDurationInDays = random.Next(7, 28); // Each module lasts between 1 and 4 weeks

                    var module = faker
                        .RuleFor(m => m.CourseId, _ => course.Id)
                        .RuleFor(m => m.StartDate, _ => currentStartDate)
                        .RuleFor(m => m.EndDate, _ =>
                        {
                            var endDate = currentStartDate.AddDays(moduleDurationInDays);
                            currentStartDate = endDate.AddDays(1); // The next module starts the day after the previous one
                            return endDate;
                        })
                        .Generate();

                    modules.Add(module);
                }
            }

            return modules;
        }

        private static List<ActivityType> GenerateActivityTypes(int count)
        {
            var faker = new Faker<ActivityType>()
                .RuleFor(at => at.Name, f => f.Commerce.ProductName());

            return faker.Generate(count);
        }

        private static List<Activity> GenerateActivities(int count, List<Module> modules, List<ActivityType> activityTypes)
        {
            var faker = new Faker<Activity>()
                .RuleFor(a => a.Name, f => f.Lorem.Sentence(2))
                .RuleFor(a => a.Description, f => f.Lorem.Paragraph())
                .RuleFor(a => a.StartDate, f => f.Date.Past())
                .RuleFor(a => a.EndDate, f => f.Date.Future())
                .RuleFor(a => a.TypeId, f => f.PickRandom(activityTypes).Id)
                .RuleFor(a => a.ModuleId, f => f.PickRandom(modules).Id);

            return faker.Generate(count);
        }

        private static async Task GenerateUsersAsync(UserManager<User> userManager, int count, List<Course> courses)
        {
            var existingUsers = await userManager.Users.ToListAsync();
            var userIds = existingUsers.Select(u => u.Id).ToHashSet();
            var faker = new Faker<User>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.CourseId, f => f.PickRandom(courses).Id);

            for (int i = 0; i < count; i++)
            {
                User user;
                do
                {
                    user = faker.Generate();
                    user.Id = Guid.NewGuid().ToString(); // Ensure a unique ID
                } while (userIds.Contains(user.Id)); // Check for uniqueness

                var result = await userManager.CreateAsync(user, "Password123!"); // Use a secure password
                if (result.Succeeded)
                {
                    // Assign roles here
                    var role = i % 2 == 0 ? "Teacher" : "Student";
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    Console.WriteLine($"Error creating user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
