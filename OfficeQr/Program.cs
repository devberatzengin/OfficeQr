using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using OfficeQr.Data;
using OfficeQr.Data.Interfaces;
using OfficeQr.Data.Repositories;
using OfficeQr.Entity;
using OfficeQr.Helpers;
using OfficeQr.Middleware;
using OfficeQr.Services;
using OfficeQr.Services.Interfaces;

namespace OfficeQr;

public class Program{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // .net can find controller class anymore
        builder.Services.AddControllers();

        builder.Services.AddHttpContextAccessor();

        // Auto Mapper
        builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

        // Custom Services
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<IAuthService,AuthService>();
        builder.Services.AddScoped<IItemService,ItemService>();
        builder.Services.AddScoped<ICabinetService,CabinetService>();
        builder.Services.AddScoped<IShelfService,ShelfService>();

        // FluentValidation validators
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        
        builder.Services.AddScoped<IValidator<OfficeQr.Dtos.Item.CreateRequest>, OfficeQr.Validators.Item.CreateRequestValidator>();
        builder.Services.AddScoped<IValidator<OfficeQr.Dtos.Item.UpdateRequest>, OfficeQr.Validators.Item.UpdateRequestValidator>();
        builder.Services.AddScoped<IValidator<OfficeQr.Dtos.Item.ReturnRequest>, OfficeQr.Validators.Item.ReturnRequestValidator>();
        
        builder.Services.AddScoped<IValidator<OfficeQr.Dtos.Cabinet.CreateRequest>, OfficeQr.Validators.Cabinet.CreateRequestValidator>();
        builder.Services.AddScoped<IValidator<OfficeQr.Dtos.Cabinet.UpdateRequest>, OfficeQr.Validators.Cabinet.UpdateRequestValidator>();
        
        builder.Services.AddScoped<IValidator<OfficeQr.Dtos.Shelf.CreateRequest>, OfficeQr.Validators.Shelf.CreateRequestValidator>();
        builder.Services.AddScoped<IValidator<OfficeQr.Dtos.Shelf.UpdateRequest>, OfficeQr.Validators.Shelf.UpdateRequestValidator>();

        // Data Dependecy Injections
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<IShelfRepository, ShelfRepository>();
        builder.Services.AddScoped<ICabinetRepository, CabinetRepository>();
        builder.Services.AddScoped<IItemShelfHistoryRepository, ItemShelfHistoryRepository>();
        builder.Services.AddScoped<IItemUserHistoryRepository, ItemUserHistoryRepository>();
        builder.Services.AddScoped<IShelfCabinetHistoryRepository, ShelfCabinetHistoryRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            };
        });


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {

            options.CustomSchemaIds(type =>
            {
                var nsSegment = type.Namespace?.Split('.').LastOrDefault();
                return string.IsNullOrEmpty(nsSegment) ? type.Name : $"{nsSegment}{type.Name}";
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "Token",
                In = ParameterLocation.Header,
                Description = "/api/auth/login yanıtındaki accessToken değerini yapıştır (Swagger 'Bearer ' önekini otomatik ekler)."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document, null)] = new List<string>()
            });
        });

        builder.Services.AddAuthorization();


        builder.Services.AddAuthentication(IdentityConstants.BearerScheme)
            .AddCookie(IdentityConstants.ApplicationScheme)
            .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddApiEndpoints();


        builder.Services.AddDbContext<IApplicationDbContext,ApplicationDbContext> (options =>
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
