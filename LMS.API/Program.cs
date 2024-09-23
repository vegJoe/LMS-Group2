using LMS.API.Data;
using LMS.API.Extensions;
using LMS.API.MappingProfile;
using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        //ToDo: AddIdentityCore
        builder.Services.AddIdentityCore<User>().AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<LMSApiContext>().AddDefaultTokenProviders();

        builder.Services.AddDbContext<LMSApiContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("LMSApiContext") ??
        throw new InvalidOperationException("Connection string 'LMSApiContext' not found.")));

        builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            await app.SeedDataAsync();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
