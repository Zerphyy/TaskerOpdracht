using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using SendGrid;
using PostmarkDotNet;
using Setup.Data;
using Setup.Models;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PostmarkDotNet.Model;
using System.Security.Cryptography;

namespace Setup.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebpageDBContext _context;

        private const string PageViews = "PageViews";

        public HomeController(ILogger<HomeController> logger, WebpageDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            SetCookieToken();
            UpdatePageViewCookie();
            return View();
        }

        public IActionResult Privacy()
        {
            UpdatePageViewCookie();
            return View();
        }
        public IActionResult Contact()
        {
            //TODO: voeg recaptcha verificatie toe, maak form submitten mogelijk, etc.
            UpdatePageViewCookie();
            return View();
        }
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var gebruikers = _context.Speler?.ToList();
                var gebruiker = gebruikers?.FirstOrDefault(g => g.Email == userId);
                if (gebruiker != null)
                {
                    var gebruikerStats = _context.SpelerStats?.ToList().FirstOrDefault(g => g.Speler == gebruiker.Email);
                    ViewBag.Naam = gebruiker.Naam;
                    ViewBag.Email = gebruiker.Email;
                    ViewBag.AantalGespeeld = gebruikerStats != null && gebruikerStats?.AantalSpellen != null ? gebruikerStats?.AantalSpellen : 0;
                    ViewBag.AantalGewonnen = gebruikerStats != null && gebruikerStats?.AantalGewonnen != null ? gebruikerStats?.AantalGewonnen : 0;
                    ViewBag.AantalVerloren = gebruikerStats != null && gebruikerStats?.AantalVerloren != null ? gebruikerStats?.AantalVerloren : 0;
                    ViewBag.WinLossRatio = gebruikerStats != null && gebruikerStats?.WinLossRatio != null ? gebruikerStats?.WinLossRatio : 0;
                    return View(gebruiker);
                }
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }
        [HttpPost]
        public async Task<IActionResult> Contact(Data.ContactData model)
        {
            var client = new HttpClient();
            var captcha = Request.Form["g-recaptcha-response"].ToString();
            var nieuwsbrief = Request.Form["Nieuwsbrief"];
            model.Nieuwsbrief = bool.Parse(nieuwsbrief);
            var parameters = new Dictionary<string, string>
                    {
                        { "secret", "6LdMr5okAAAAAMgtagSFUA0ukkzsloNTQAcsHMI_" },
                        { "response", captcha }
                    };
            var content = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RecaptchaResponse>(responseBody);
            //result = recaptcha verifyen, modelState = eisen gesteld aan values in form
            if (!result.Success || !ModelState.IsValid)
            {
                //wanneer 1 van de 2 niet correct is, view terug sturen en fouten aanpassen
                return View(model);
            }
            else
            {
                //sendmail shit
                await SendMail(model.Email, model.Naam, model.Onderwerp, model.Phone, model.Bericht, model.Nieuwsbrief, model.Bellen);
                //db connectie opzetten
                using (_context)
                {
                    //data toevoegen aan ContactData tabel
                    _context.ContactData.Add(model);
                    //db opslaanS
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            UpdatePageViewCookie();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public void SetCookieToken()
        {

            if (HttpContext.Session.GetString("token") == null)
            {
                string input = "123456";
                HttpContext.Session.SetString("token", input);
            }

        }

        public void UpdatePageViewCookie()
        {
            var currentCookieValue = Request.Cookies[PageViews];
            if (currentCookieValue == null)
            {
                Response.Cookies.Append(PageViews, "1");
            }
            else
            {
                var newCookieValue = short.Parse(currentCookieValue) + 1;

                Response.Cookies.Append(PageViews, newCookieValue.ToString());
            }
        }
        static async Task SendMail(string email, string naam, string onderwerp, string nummer, string bericht, bool nieuwsbrief, DateTime? bellen)
        {
            var contactOpnemen = bellen != default(DateTime) ? $"je kan me vanaf {bellen} bereiken via {nummer}" : "Gelieve mij niet te bellen";
            //var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            //var apiKey = "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";
            //var client = new SendGridClient(apiKey);
            //var from = new EmailAddress(email, naam);
            //var subject = onderwerp;
            //var to = new EmailAddress("kevinspijker@kpnmail.nl", "Kevin Spijker");
            //var plainTextContent = $"{bericht}, {contactOpnemen}, ik wil graag {(nieuwsbrief == true ? "wel een" : "geen")} nieuwsbrief ontvangen";
            //var htmlContent = $"<strong>{onderwerp}</strong> <br> {bericht} <br><br> {contactOpnemen} <br><br> ik wil graag {(nieuwsbrief == true ? "wel een" : "geen")} nieuwsbrief ontvangen";
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //var response = await client.SendEmailAsync(msg);
            // Send an email asynchronously:
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
                Subject = onderwerp,
                TextBody = "body",
                HtmlBody = "Geachte " + naam + "<br> We hebben je contactformulier met bericht: <br>" + bericht + " <br> in goede orde ontvangen, ook heb je het volgende aangegeven<br>" + contactOpnemen + "<br> en als laatst heb je aangegeven je nieuwsbrief status op " + nieuwsbrief + " te willen zetten",
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
}