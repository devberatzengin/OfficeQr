using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeQr.Data;
using OfficeQr.Entity;
using OfficeQr.Helpers;
using Scalar.AspNetCore;

namespace OfficeQr;

public class Program{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme)
            .AddBearerToken(IdentityConstants.BearerScheme);
        builder.Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();
        
        builder.Services.AddDbContext<ApplicationDbContext> (options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.ApplyMigrations();
        }

        app.UseHttpsRedirection();

        app.MapIdentityApi<User>();

        await app.RunAsync();
    }
}
