using Bogus;
using LMS.API.Models.Entities;
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

                await db.Database.MigrateAsync();

                if (await db.Activities.AnyAsync()) return; // If data exists, don't seed again

                try
                {
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
                    var users = GenerateUsers(10, courses);
                    await db.AddRangeAsync(users);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message); // Log error for debugging
                    throw;
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
            var faker = new Faker<Module>()
                .RuleFor(m => m.Name, f => f.Lorem.Word())
                .RuleFor(m => m.Description, f => f.Lorem.Sentence())
                .RuleFor(m => m.CourseId, f => f.PickRandom(courses).Id);

            return faker.Generate(count);
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

        private static List<User> GenerateUsers(int count, List<Course> courses)
        {
            var faker = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.CourseId, f => f.PickRandom(courses).Id);

            return faker.Generate(count);
        }
    }
}
