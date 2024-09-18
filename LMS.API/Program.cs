
using Companies.API.Extensions;
using LMS.API.Data;
using LMS.API.MappingProfile;
using LMS.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        builder.Services.AddSwaggerGen();

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
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        //ToDo: AddAuthentication
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
