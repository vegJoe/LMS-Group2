using AutoMapper;
using LMS.API.Controllers;
using LMS.API.Data;
using LMS.API.Models.Entities;
using LMS.API.MappingProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controller.Tests.Fixtures
{
    public class CombinedFixture : IDisposable
    {
        public Mock<UserManager<User>> MockUserManager { get; }
        public CoursesController Sut { get; }
        public LMSApiContext Context { get; }
        public Mapper Mapper { get; }

        public CombinedFixture()
        {
            Mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            }));

            var options = new DbContextOptionsBuilder<LMSApiContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestDataBase;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            Context = new LMSApiContext(options);
            Context.Database.Migrate();

            SeedDatabase(Context);

            var mockUserStore = new Mock<IUserStore<User>>();
            MockUserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            Sut = new CoursesController(Context, Mapper);
        }

        private void SeedDatabase(LMSApiContext context)
        {
            context.Courses.AddRange(
                new List<Course>
                {
                    new Course
                    {
                        Name = "C# .Net",
                        Description = "Learn the basics of programming.",
                        StartDate = DateTime.UtcNow
                    },
                    new Course
                    {
                        Name = "Backend",
                        Description = "An in-depth look at API.",
                        StartDate = DateTime.UtcNow.AddDays(7)
                    }
                });

            context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
