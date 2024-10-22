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
                using (_context)
                {
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
                await SendMail(userMail, nummer);
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
                        // Set a longer expiration time for persistent login
                        properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30); // e.g., 30 days
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
        static async Task SendMail(string email, int nummer)
        {
            var headerDictionary = new Dictionary<string, string>()
{
    { "X-CUSTOM-HEADER", "Header content" }
};

            // Create a HeaderCollection using the dictionary
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
            var sendResult = await client.SendMessageAsync(message);

            if (sendResult.Status == PostmarkStatus.Success) { }
            else { /* Resolve issue.*/ }
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
