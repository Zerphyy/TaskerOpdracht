using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Setup;
using Setup.Data;
using Setup.Hubs;
using System.Runtime.InteropServices;
bool staysLoggedIn = false;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
    builder =>
    {
        builder.WithOrigins(
            "https://cdnjs.cloudflare.com",   // SignalR
            "https://code.jquery.com",         // jQuery
            "https://www.gstatic.com",         // reCAPTCHA
            "https://www.google.com"           // reCAPTCHA
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
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
    options.Cookie.Name = "__Host-Cookie";
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' https://www.google.com/recaptcha/ https://www.gstatic.com/recaptcha/ https://cdnjs.cloudflare.com; style-src 'self' 'unsafe-inline'; img-src 'self' data:; object-src 'none'; frame-src https://www.google.com/recaptcha/ https://recaptcha.google.com/recaptcha/; connect-src 'self' http://localhost:* ws://localhost:* wss://localhost:*");
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