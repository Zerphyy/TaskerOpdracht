using Microsoft.AspNetCore.Mvc;
using Setup.Data;

namespace Setup.Controllers
{
    public class CVController : Controller
    {
        private const string PageViews = "PageViews";
        public IActionResult Index()
        {
            UpdatePageViewCookie();
            var persoon = new Personalia()
            {
                Naam = "Linde Eikenboom",
                Email = "sandravangennep@gmail.nl",
                Telefoonnummer = "+31 (0)6 12345678",
                Straat = "Diemerkade 1",
                Zip = "1111 AA Diemen",
                Verjaardag = new DateTime(1989, 07, 21),
                LinkedIn = "linkedin.com/sandravangennep",
                BIG = 5211345,
                Eigenschappen = new List<string> { "Secuur werken", "Flexibel", "Goed om kunnen gaan met olifanten" },
                Vaardigheden = new Dictionary<string, string> { { "Farmaceutische deskundigheid", "★★★★☆" }, { "ChipSoft", "★☆☆☆☆" }, { "Pharmalink", "★★★☆☆" }, { "Leidinggevende Kwaliteiten", "★★☆☆☆" } },
                Profiel = new Profiel()
                {
                    Beschrijving = "Als arts en developer ben ik gekwalificeerd om de medische context te onderzoeken en een technische oplossing te vinden die daarbij past. Ik heb veel kennis en ervaring op het gebied van medische en technische zaken, waardoor ik in staat ben om complexe problemen te analyseren en te begrijpen. Dit maakt me een goede kandidaat om problemen op te lossen die betrekking hebben op zowel de medische als technische aspecten. Bovendien ben ik in staat om deze problemen op een effectieve en efficiënte manier op te lossen aangezien ik bekwaam ben in zowel de medische als technische zaken. Hierdoor ben ik in staat om zo snel mogelijk een oplossing te bieden om problemen op te lossen.",
                    Opleidingen = new List<Opleiding> { new Opleiding() { Naam = "Farmacie", Plaats = "Universiteit Utrecht, Utrecht", Van = DateTime.Now.AddYears(-1), Tot = DateTime.Now },
                                                        new Opleiding() {Naam = "Onderzoeker", Plaats = "Lindeboom instituut, Utrecht", Van = DateTime.Now.AddYears(-3), Tot = DateTime.Now.AddYears(-2)}
                                                      },
                    Stages = null,
                    Cursussen = null,
                    Werkervaringen = null
                }
            };
            return View(persoon);
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
    }
}
