using Controller.Tests.Extensions;
using Controller.Tests.Fixtures;
using LMS.API.Controllers;
using LMS.API.Data;
using LMS.API.Models.Dtos;
using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Controller.Tests
{
    public class CoursesControllerTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture fixture;

        public CoursesControllerTests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }


        [Fact]
        public async Task CreateCourse_ShouldReturn201Created()
        {
            var dto = new CreateUpdateCourseDto
            {
                Name = "Course",
                Description = "Course Description",
                StartDate = DateTime.UtcNow,
            };

            var result = await fixture.Sut.CreateCourse(dto);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task GetCourses_ShouldReturnExpectedResponse()
        {
            var result = await fixture.Sut.GetCourses();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var response = okResult.Value;

            var totalCoursesProperty = response.GetType().GetProperty("TotalCourses");
            var coursesProperty = response.GetType().GetProperty("Courses");

            Assert.NotNull(totalCoursesProperty);
            Assert.NotNull(coursesProperty);

            int totalCourses = (int)totalCoursesProperty.GetValue(response);
            var courses = coursesProperty.GetValue(response) as IEnumerable<CourseDto>;

            Assert.IsAssignableFrom<IEnumerable<CourseDto>>(courses);

            Assert.Equal(2, totalCourses);
            Assert.NotEmpty(courses);
        }
    }
}
