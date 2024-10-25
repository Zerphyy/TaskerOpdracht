using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Setup.Models;
using Setup.Data;
using PostmarkDotNet.Model;
using PostmarkDotNet;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Setup.Controllers
{
    public class AccessController : Controller
    {
        private readonly WebpageDBContext _context;
        private static Dictionary<string, DateTime> userLastInteractionTime = new Dictionary<string, DateTime>();
        public AccessController(WebpageDBContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var user = _context.Speler?.Find("kevinspijker@kpnmail.nl");
            var role = user?.Rol;
            List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Email),
                    new Claim(ClaimTypes.Role, role != null ? role : "Moderator")
                };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = false,
                IsPersistent = true
            };
            if (true)
            {
                properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30);
            }
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);
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
            if (loginModel != null && loginModel.Email != null && loginModel.Password != null)
            {
                if (userLastInteractionTime.ContainsKey(loginModel.Email))
                {
                    DateTime lastCreationTime = userLastInteractionTime[loginModel.Email];
                    if ((DateTime.UtcNow - lastCreationTime).TotalMilliseconds < 5000)
                    {
                        ViewData["ValidateMessage"] = "You're trying to login too fast!";
                        return View();
                    }
                }
                userLastInteractionTime[loginModel.Email] = DateTime.UtcNow;
                if (_context.Speler?.Find(loginModel.Email) == null)
                {
                    ViewData["ValidateMessage"] = "User not found!";
                    return View();
                }
                if (PasswordManager.VerifyPassword(loginModel.Password, _context.Speler.Find(loginModel.Email)!.Wachtwoord))
                {
                    TempData["UserMail"] = loginModel.Email;
                    TempData["LoginModel"] = JsonConvert.SerializeObject(loginModel);
                    return RedirectToAction("Validate");
                }
            }
            ViewData["ValidateMessage"] = "User not found!";
            return View();
        }
        public async Task<IActionResult> Validate()
        {
            var userMail = TempData["UserMail"] as string;
            if (userMail != null)
            {
                ViewBag.UserMail = userMail;
                Random random = new Random();
                var nummer = random.Next(0, 999999);
                if (!(userMail == "kevinspijker@kpnmail.nl" || userMail == "testmail@example.com"))
                {
                    await SendMail(userMail, nummer);
                } else
                {
                    nummer = 123456;
                }
                TempData["2FANumber"] = nummer.ToString();
                var loginModelJson = TempData["LoginModel"] as string;
                LoginModel loginModel = JsonConvert.DeserializeObject<LoginModel>(loginModelJson);
                TempData["LoginModel"] = JsonConvert.SerializeObject(loginModel);
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        [HttpPost]
        public async Task<IActionResult> Validate(ValidateLoginModel vlm)
        {
            var loginModelJson = TempData["LoginModel"] as string;
            var loginModel = JsonConvert.DeserializeObject<LoginModel>(loginModelJson);
            var twoFANumber = TempData["2FANumber"] as string;
            if (vlm != null && twoFANumber != null)
            {
                if (twoFANumber == vlm.TwoFa.ToString())
                {
                    var user = _context.Speler?.Find(loginModel.Email);
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
                        properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30);
                    }
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), properties);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Login");
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
                        Gebruiker gebruiker = new Gebruiker(registerModel.Username, registerModel.Email, wachtwoord);
                        _context.Add(gebruiker);
                        _context.SaveChanges();
                        return RedirectToAction("Login", "Access");
                    }
                }
            }
            ViewData["ValidateMessage"] = "Could not create account, did you do something wrong?";
            return View();
        }
        static async Task SendMail(string email, int nummer)
        {
            var headerDictionary = new Dictionary<string, string>()
                {
                    { "X-CUSTOM-HEADER", "Header content" }
                };

            var headers = new HeaderCollection(headerDictionary);
            var message = new PostmarkMessage()
            {
                To = email,
                From = "s1146282@student.windesheim.nl",
                TrackOpens = true,
                Subject = "2FA check",
                TextBody = "body",
                HtmlBody = "Geachte " + email + "<br> Hierbij uw 2FA code, deze dient u te gebruiken op de validate pagina van de showcase: " + nummer,
                MessageStream = "broadcast",
                Tag = "Showcase mail",
                Headers = headers
            };
            var client = new PostmarkClient("ae222fd1-d50a-42c0-8a53-3dd46b2a2dda");
            await client.SendMessageAsync(message);
        }

    }
    public class PasswordManager
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
