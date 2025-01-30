using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using NpgsqlTypes;
using Server.Contexts;
using Server.Entities;
using Server.Repositories;
using Swashbuckle.AspNetCore.Filters;

// MUST HAVE IT LIKE THIS FOR NLOG TO RECOGNIZE DOTNET USER-SECRETS INSTEAD OF HARDCODED DELIMIT PLACEHOLDER VALUE FROM APPSETTINGS.JSON

// #if DEBUG
//     var logger = LogManager.Setup().LoadConfigurationFromFile("nlog_dev.config").GetCurrentClassLogger();
// #else
//     var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
// #endif


// try
// {
    
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // builder.Logging.ClearProviders();
    // builder.Host.UseNLog();


    builder.Services.AddControllers();
    builder.Services.AddRazorPages();
    builder.Services.AddHttpClient();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options=>{
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
        });

        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });

    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<IRolesRepository, RolesRepository>();
    builder.Services.AddScoped<IUsersRepository, UsersRepository>();

    // Configure the Identity database context
    builder.Services.AddDbContext<UserManagementContext>(options =>
        options.UseNpgsql(
            builder.Environment.IsDevelopment()
                    ?
                        builder.Configuration.GetConnectionString("UserManagementDB")
                    :
                        Environment.GetEnvironmentVariable("UserManagementDB"))
            );


    builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<UserManagementContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders()
    .AddRoles<ApplicationRole>();

    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequiredLength = 7;
        options.Password.RequiredUniqueChars = 3;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = true;
        options.SignIn.RequireConfirmedEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    });

    builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Environment.IsDevelopment() ? builder.Configuration["Jwt:Issuer"] : Environment.GetEnvironmentVariable("Jwt_Issuer"),
            ValidAudience = builder.Environment.IsDevelopment() ? builder.Configuration["Jwt:Audience"] : Environment.GetEnvironmentVariable("Jwt_Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Environment.IsDevelopment() ? builder.Configuration["Jwt:Key"] : Environment.GetEnvironmentVariable("Jwt_Key")))
        };
    });

    if (!builder.Environment.IsDevelopment())
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
        builder.WebHost.UseUrls($"http://*:{port}");
    }

    // add rate limits
    builder.Services.AddRateLimiter(options => {
        options.AddFixedWindowLimiter("FixedPolicy", options => {
            options.PermitLimit = 5;
            options.Window = TimeSpan.FromSeconds(30);
            // options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 0;
        });
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        
        // for mvc apps:
        // options.OnRejected = (ctx, token) => {
        //     ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        //     ctx.HttpContext.Response.WriteAsync("no more than one call every 5 seconds!");
        //     return ValueTask.CompletedTask;
        // };
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();
    app.UseRouting();

    // MUST be after app.userouting
    // https://github.com/dotnet/aspnetcore/issues/45302
    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");


    app.Run();
// }
//  catch (Exception ex)
// {
//     logger.Error(ex, "Stopped program because of exception: " + ex);
//     throw ex;
// }
// finally {
//     LogManager.Shutdown();
// }