

/*

dotnet ef dbcontext scaffold "" Npgsql.EntityFrameworkCore.PostgreSQL -o Contexts
*/

using Microsoft.AspNetCore.Identity;
using DotnetUserManagementSystem.Contexts;
using Microsoft.EntityFrameworkCore;

TimeSpan sessionExpiration = TimeSpan.FromHours(1);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = sessionExpiration;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<UserManagementContext>(options =>
    options.UseNpgsql(
        builder.Environment.IsDevelopment()
                ?
                    builder.Configuration.GetConnectionString("UserManagementDB")
                :
                    Environment.GetEnvironmentVariable("UserManagementDB"))
        );

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
.AddEntityFrameworkStores<UserManagementContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 7;
    options.Password.RequiredUniqueChars = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
});

if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
    builder.WebHost.UseUrls($"http://*:{port}");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseDeveloperExceptionPage();
app.UseSession();


app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
