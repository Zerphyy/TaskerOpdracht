using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Setup.Models;
using Setup.Data;

namespace Setup.Controllers
{
    public class AccessController : Controller
    {
        private readonly WebpageDBContext _context;
        public AccessController(WebpageDBContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal userLoggedIn = HttpContext.User;
            if (userLoggedIn.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            //if (loginModel.Email == "kevinspijker@kpnmail.nl" &&
            //    loginModel.Password == "welkom01" && loginModel.Username != null)
            //{
                
            //    return RedirectToAction("Index", "Home");
            //}
            if (loginModel != null && loginModel.Email != null && loginModel.Password != null)
            {
                using (_context)
                {
                    if (_context.Speler?.Find(loginModel.Email) == null)
                    {
                        ViewData["ValidateMessage"] = "User not found!";
                        return View();
                    }
                    if (PasswordManager.VerifyPassword(loginModel.Password, _context.Speler.Find(loginModel.Email)!.Wachtwoord))
                    {
                        var user = _context.Speler.Find(loginModel.Email);
                        var role = user?.Rol;
                        List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, loginModel.Email),
                    new Claim(ClaimTypes.Role, role != null ? role : "Gebruiker")
                };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);

                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = false,
                            IsPersistent = loginModel.StayLoggedIn
                        };
                        if (loginModel.StayLoggedIn)
                        {
                            // Set a longer expiration time for persistent login
                            properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30); // e.g., 30 days
                        }
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), properties);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ViewData["ValidateMessage"] = "User not found!";
            return View();
        }
        public IActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                if (registerModel.Email != null && registerModel.Password != null)
                {
                    string wachtwoord = PasswordManager.HashPassword(registerModel.Password);
                    if (PasswordManager.VerifyPassword(registerModel.Password, wachtwoord))
                    {
                        await using (_context)
                        {
                            Gebruiker gebruiker = new Gebruiker(registerModel.Username, registerModel.Email, wachtwoord);
                            _context.Add(gebruiker);
                            _context.SaveChanges();
                        }
                        return RedirectToAction("Login", "Access");
                    }
                }
            }
            ViewData["ValidateMessage"] = "Could not create account, did you do something wrong?";
            return View();
        }
        
    }
    public class PasswordManager
    {
        public static string HashPassword(string password)
        {
            // Hash the password using BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Verify the entered password against the stored hash
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
