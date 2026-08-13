using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeQr.Data;
using OfficeQr.Data.Interfaces;
using OfficeQr.Data.Repositories;
using OfficeQr.Entity;
using OfficeQr.Helpers;
using OfficeQr.Middleware;
using OfficeQr.Services;
using OfficeQr.Services.Interfaces;
using Scalar.AspNetCore;

namespace OfficeQr;

public class Program{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // .net can find controller class anymore
        builder.Services.AddControllers();

        // Custom Services
        builder.Services.AddScoped<IAuthService,AuthService>();

        // Data Dependecy Injections
        builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<IShelfRepository, ShelfRepository>();
        builder.Services.AddScoped<ICabinetRepository, CabinetRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthorization();


        builder.Services.AddAuthentication(IdentityConstants.BearerScheme)
            .AddCookie(IdentityConstants.ApplicationScheme)
            .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddApiEndpoints();
        
        
        builder.Services.AddDbContext<ApplicationDbContext> (options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        

        var app = builder.Build();

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.ApplyMigrations();

        await app.SeedIdentityRolesAsync();

        if (app.Environment.IsDevelopment())
        {
            await app.SeedDevelopmentAdminAsync();
        }

        app.UseHttpsRedirection();
   
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
        app.MapGroup("/identity").MapIdentityApi<User>().ExcludeFromDescription();
        
        await app.RunAsync();
    }
}
