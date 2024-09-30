using AutoMapper;
using LMS.API.Controllers;
using LMS.API.Data;
using LMS.API.Models.Entities;
using LMS.API.MappingProfile;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Controller.Tests.Fixtures
{
    public class IntegrationTestFixture : IDisposable
    {
        public CoursesController Sut { get; }
        public LMSApiContext Context { get; }
        public Mapper Mapper { get; }

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
            context.Courses.AddRange(
                new List<Course>
                {
                    new Course
                    {
                        Name = "C# .Net",
                        Description = "Learn the basics of programming.",
                        StartDate = DateTime.Parse("2023-10-25 12:09:23"),
                    },
                    new Course
                    {
                        Name = "Backend",
                        Description = "An in-depth look at API.",
                        StartDate = DateTime.Parse("2024-09-11 00:44:43"),
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
