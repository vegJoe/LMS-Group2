using LMS.API.Data;
using LMS.API.Extensions;
using LMS.API.MappingProfile;
using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace LMS.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.ConfigureJwt(builder.Configuration);
        builder.Services.ConfigureCors();
        builder.Services.ConfigureServices();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "LMS Api",
                Description = "A ASP.NET Core Web API for Learning Management System.",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Group 2",
                    Email = string.Empty,
                },
                License = new OpenApiLicense
                {
                    Name = "Use under LICX",
                    Url = new Uri("https://example.com/license")
                }
            });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            // Add security definition for Bearer token
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter the JWT Bearer token.",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            // Add security requirement for Bearer token
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        //ToDo: AddIdentityCore
        builder.Services.AddIdentityCore<User>().AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<LMSApiContext>().AddDefaultTokenProviders();

        //Add Auth Options
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireTeacherRole", policy => policy.RequireRole("Teacher"));
            options.AddPolicy("RequireStudentRole", policy => policy.RequireRole("Student"));
        });

        builder.Services.AddDbContext<LMSApiContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("LMSApiContext") ??
        throw new InvalidOperationException("Connection string 'LMSApiContext' not found.")));

        builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS Api");
            });
            await app.SeedDataAsync();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
