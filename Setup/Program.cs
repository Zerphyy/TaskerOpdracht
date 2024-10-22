using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Setup;
using Setup.Data;
using Setup.Hubs;
using System.Runtime.InteropServices;
bool staysLoggedIn = false;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
ConfigureServices(builder.Services);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Access/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<WebpageDBContext>(options =>
    {
        string connectionString;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            connectionString = builder.Configuration.GetConnectionString("CheckersChampsDb");
        }
        else
        {
            connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        }

        options.UseSqlServer(connectionString);
    });

    services.AddSignalR();
    services.AddControllers();
}
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    await next();
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseMiddleware<UserRoleMiddleware>();

app.MapHub<GameHub>("/gameHub");
app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();