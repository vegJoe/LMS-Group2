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
                    var activityTypes = GenerateActivityTypes();
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
            var courses = new List<Course>
        {
            new Course { Name = "Web Development", Description = "Learn how to build modern web applications", StartDate = DateTime.Now.AddMonths(-3) },
            new Course { Name = "Backend Development", Description = "Master server-side programming and databases", StartDate = DateTime.Now.AddMonths(-4) },
            new Course { Name = "Machine Learning", Description = "Dive into AI with practical machine learning techniques", StartDate = DateTime.Now.AddMonths(-2) },
            new Course { Name = "Mobile App Development", Description = "Create powerful mobile applications", StartDate = DateTime.Now.AddMonths(-5) },
            new Course { Name = "Cloud Computing", Description = "Learn cloud architecture and deployment", StartDate = DateTime.Now.AddMonths(-6) }
        };

            return courses;
        }

        private static List<Module> GenerateModules(int count, List<Course> courses)
        {
            var modules = new List<Module>();

            // Define relevant modules for each course
            var courseModulesDict = new Dictionary<string, List<string>>
    {
        { "Web Development", new List<string> { "HTML & CSS", "JavaScript Basics", "Frontend Frameworks", "APIs and REST", "Web Security" } },
        { "Backend Development", new List<string> { "Databases", "APIs and Microservices", "Authentication", "Server-Side Languages", "DevOps Fundamentals" } },
        { "Machine Learning", new List<string> { "Data Preprocessing", "Supervised Learning", "Unsupervised Learning", "Neural Networks", "Model Deployment" } },
        { "Mobile App Development", new List<string> { "Introduction to Mobile Apps", "Android Development", "iOS Development", "Cross-Platform Development", "App Deployment" } },
        { "Cloud Computing", new List<string> { "Cloud Fundamentals", "AWS Basics", "Azure Overview", "Google Cloud Platform", "Cloud Security" } }
    };

            DateTime today = DateTime.Parse("2024-10-06"); // Fixed current date

            foreach (var course in courses)
            {
                if (courseModulesDict.TryGetValue(course.Name, out var courseModules))
                {
                    // Create 2 modules in the past
                    for (int i = 0; i < 2; i++)
                    {
                        var moduleName = courseModules[i]; // Get module name from the list

                        modules.Add(new Module
                        {
                            Name = moduleName,
                            Description = $"Detailed course material on {moduleName}",
                            CourseId = course.Id,
                            StartDate = today.AddDays(-(30 * (i + 1))), // Start 30 or 60 days ago
                            EndDate = today.AddDays(-(30 * (i + 1)) + 7) // End 7 days after start
                        });
                    }

                    // Create 3 active modules
                    for (int i = 2; i < 5; i++) // Start from index 2 for active modules
                    {
                        var moduleName = courseModules[i]; // Get module name from the list

                        modules.Add(new Module
                        {
                            Name = moduleName,
                            Description = $"Detailed course material on {moduleName}",
                            CourseId = course.Id,
                            StartDate = today.AddDays(7 * (i - 1)), // Start 7 days from today for each module
                            EndDate = today.AddDays(7 * (i - 1) + 7) // Each module lasts 1 week
                        });
                    }
                }
            }

            return modules;
        }


        private static List<ActivityType> GenerateActivityTypes()
        {
            var activityTypes = new List<ActivityType>
        {
            new ActivityType { Name = "E-Learning" },
            new ActivityType { Name = "Assignment" },
            new ActivityType { Name = "Presentation" },
            new ActivityType { Name = "Group Work" },
            new ActivityType { Name = "Quiz" }
        };

            return activityTypes;
        }

        private static List<Activity> GenerateActivities(int count, List<Module> modules, List<ActivityType> activityTypes)
        {
            var activities = new List<Activity>();
            var faker = new Faker();

            foreach (var module in modules)
            {
                var activityDescriptions = new Dictionary<string, string>
            {
                { "E-Learning", "Participate in the online learning module." },
                { "Assignment", "Complete the assignment based on the module's learning materials." },
                { "Presentation", "Prepare and deliver a presentation on the module's topic." },
                { "Group Work", "Collaborate with peers on a project." },
                { "Quiz", "Take a short quiz to test your understanding of the module." }
            };

                for (int i = 0; i < 5; i++) // Generates 5 activities per module
                {
                    var activityType = activityTypes[i % activityTypes.Count]; // Ensures activity types cycle through

                    var activity = new Activity
                    {
                        Name = $"{activityType.Name} for {module.Name}",
                        Description = activityDescriptions[activityType.Name], // Access the updated descriptions
                        StartDate = module.StartDate.AddDays(faker.Random.Int(1, 3)),
                        EndDate = module.EndDate.AddDays(faker.Random.Int(4, 7)),
                        TypeId = activityType.Id,
                        ModuleId = module.Id
                    };

                    activities.Add(activity);
                }
            }

            return activities;
        }




        private static async Task GenerateUsersAsync(UserManager<User> userManager, int count, List<Course> courses)
        {
            var existingUsers = await userManager.Users.ToListAsync();
            var userIds = existingUsers.Select(u => u.Id).ToHashSet();

            var firstNames = new[] { "John", "Jane", "Michael", "Sarah", "David", "Emma", "Chris", "Sophia", "Daniel", "Olivia" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };

            var faker = new Faker<User>()
                .RuleFor(u => u.FirstName, f => f.PickRandom(firstNames))  // Use predefined first names
                .RuleFor(u => u.LastName, f => f.PickRandom(lastNames))    // Use predefined last names
                .RuleFor(u => u.Email, (f, u) => $"{u.FirstName}.{u.LastName}@lms.com".ToLower())  // Custom email format
                .RuleFor(u => u.UserName, (f, u) => $"{u.FirstName}.{u.LastName}".ToLower())        // Username based on name
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())    // Generate realistic phone numbers
                .RuleFor(u => u.CourseId, f => f.PickRandom(courses).Id);   // Random course assignment

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
                    var role = i % 2 == 0 ? "Teacher" : "Student";  // Alternate between Teacher and Student roles
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
