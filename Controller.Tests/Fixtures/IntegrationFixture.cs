using AutoMapper;
using LMS.API.Controllers;
using LMS.API.Data;
using LMS.API.Models.Entities;
using LMS.API.MappingProfile;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bogus;

namespace Controller.Tests.Fixtures
{
    public class IntegrationTestFixture : IDisposable
    {
        public CoursesController Sut { get; }
        public LMSApiContext Context { get; }
        public Mapper Mapper { get; }
        public DateTime startDateForTest { get; private set; }

        public IntegrationTestFixture()
        {
            Mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            }));

            var options = new DbContextOptionsBuilder<LMSApiContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryTestDb")
                .Options;

            Context = new LMSApiContext(options);

            SeedDatabase(Context);

            Sut = new CoursesController(Context, Mapper);
        }

        private void SeedDatabase(LMSApiContext context)
        {
            startDateForTest = DateTime.Now;

            context.Courses.AddRange(
                new List<Course>
                {
                    new Course
                    {
                        Name = "C# .Net",
                        Description = "Learn the basics of programming.",
                        StartDate = startDateForTest,
                    },
                    new Course
                    {
                        Name = "Backend",
                        Description = "An in-depth look at API.",
                        StartDate = DateTime.Parse("2024-09-11 00:44:43"),
                    }
                });

            context.SaveChanges();

            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context), null, null, null, null);
            var userManager = new UserManager<User>(
                new UserStore<User>(context), null, null, null, null, null, null, null, null);

            SeedUsersAndRoles(context, userManager, roleManager);
        }

        private async Task SeedUsersAndRoles(LMSApiContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create "Teacher" and "Student" roles
            string[] roleNames = { "Teacher", "Student" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))  // Await this call
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));  // Await this call
                }
            }

            // Use Bogus to generate a user and assign the "Teacher" role
            var faker = new Faker<User>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());

            var teacherUser = faker.Generate();
            teacherUser.Id = Guid.NewGuid().ToString(); // Ensure a unique ID

            // Create the teacher user with a password
            var result = await userManager.CreateAsync(teacherUser, "Password123!");  // Await this call

            await userManager.AddToRoleAsync(teacherUser, "Teacher");  // Await this call
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
