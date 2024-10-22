using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.SignalR;
using Setup.Controllers;
using Setup.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace TestShowcase.ControllerTests
{
    [TestFixture]
    public class DammenControllerTests
    {
        private DammenController _controller;
        private Mock<IHubContext<GameHub>> _mockHubContext;
        private WebpageDBContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _mockHubContext = new Mock<IHubContext<GameHub>>();

            var options = new DbContextOptionsBuilder<WebpageDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new WebpageDBContext(options);

            SeedDatabase(_dbContext);

            _controller = new DammenController(_mockHubContext.Object, _dbContext);
        }

        private void SeedDatabase(WebpageDBContext context)
        {
            context.Speler?.Add(new Gebruiker { Naam = "Zerphy", Email = "kevinspijker@kpnmail.nl", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password", BCrypt.Net.BCrypt.GenerateSalt()), Rol = "Moderator" });
            context.Speler?.Add(new Gebruiker { Naam = "Zerphy2", Email = "kevinspijker2@kpnmail.nl", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password1", BCrypt.Net.BCrypt.GenerateSalt()), Rol = "Moderator" });

            context.SaveChanges();
        }
        [Test]
        public void RetrieveIndexView()
        {
            var result = _controller.Index();

            Assert.IsNotNull(result);
        }
        [Test]
        public void RetrieveCreateView()
        {
            var result = _controller.Create();

            Assert.IsNotNull(result);
        }
        [Test]
        public void CreateGameTest()
        {
            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            var result = _controller.Create(newGame);

            var addedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.IsNotNull(addedGame);
            Assert.That("Spel1", Is.EqualTo(addedGame.SpelNaam));
        }
        [Test]
        public async Task CreateGame_RateLimitTest()
        {
            DamSpel newGame1 = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");
            DamSpel newGame2 = new DamSpel(0, "Spel2", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            var firstResult = await _controller.Create(newGame1);

            var addedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.IsNotNull(addedGame, "The first game was not added to the database.");

            var secondResult = await _controller.Create(newGame2);

            Assert.IsInstanceOf<JsonResult>(secondResult, "The result should be a JsonResult.");
            var jsonResponse = secondResult as JsonResult;

            string jsonString = JsonConvert.SerializeObject(jsonResponse.Value);

            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

            Assert.IsNotNull(jsonData, "The JsonResult data should not be null.");
            Assert.IsTrue(jsonData.ContainsKey("success"), "JsonResult should contain a 'success' key.");
            Assert.IsTrue(jsonData.ContainsKey("message"), "JsonResult should contain a 'message' key.");

            Assert.IsFalse((bool)jsonData["success"], "The second game creation should have been blocked due to rate limiting.");
            Assert.AreEqual("You can only create one game per second.", jsonData["message"].ToString(), "The message for rate limiting is incorrect.");

            var addedGame2 = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel2");
            Assert.IsNull(addedGame2, "The second game should not have been added to the database.");
        }




        [Test]
        public void RetrieveGameView()
        {
            var result = _controller.Spel(1);

            Assert.IsNotNull(result);
        }
        [Test]
        public async Task DeleteGameTest()
        {
            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            await _controller.Create(newGame);

            DamSpel addedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.IsNotNull(addedGame, "The game was not added to the database.");

            var result = _controller.Delete(addedGame);

            Assert.IsNotNull(result);

            DamSpel deletedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.Id == addedGame.Id);
            Assert.IsNull(deletedGame, "The game was not deleted from the database.");
        }
        [Test]
        public async Task ViewGameTest()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "kevinspijker@kpnmail.nl"),
        new Claim(ClaimTypes.Name, "Kevin Spijker")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            await _controller.Create(newGame);

            DamSpel addedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.IsNotNull(addedGame, "The game was not added to the database.");

            var result = _controller.Spel(addedGame.DamBordId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);

            var viewResult = result as ViewResult;
            Assert.AreEqual(JsonConvert.SerializeObject(new string[] { newGame.Creator, "" }), viewResult.ViewData["Spelers"]);
            Assert.AreEqual("kevinspijker@kpnmail.nl", viewResult.ViewData["Gebruiker"]);
        }

        [Test]
        public async Task UpdateBoardTest()
        {
            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            await _controller.Create(newGame);

            DamSpel addedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.IsNotNull(addedGame, "The game was not added to the database.");

            var result = _controller.UpdateBoardData("1234567890", addedGame.Id.ToString(), addedGame.AanZet == addedGame.Creator ? addedGame.Deelnemer : addedGame.Creator);  // Assuming your Delete method takes an ID
            DamSpel updatedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.AreNotEqual(updatedGame.BordStand, "0101010110101010010101010000000000000000202020200202020220202020");
        }
        [Test]
        public async Task ProcessWinAndLossTest()
        {
            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            await _controller.Create(newGame);

            DamSpel addedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.IsNotNull(addedGame, "The game was not added to the database.");

            GameData data = new GameData
            {
                Speler1 = "adsa",
                Speler2 = "asdcvx",
                DamSpel = addedGame
            };

            _controller.AddPlayerToGame(data);

            DamSpel updatedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");

            string[] spelers = new string[2];
            spelers[0] = updatedGame.Creator;
            spelers[1] = updatedGame.Deelnemer;

            var result = _controller.ProcessWin(updatedGame.Id.ToString(), updatedGame.Deelnemer, spelers, updatedGame.Deelnemer);

            Assert.IsNotNull(result);
        }
        [Test]
        public async Task GetGameListEmpty()
        {
            var result = _controller.GetGameLijst();

            Assert.IsNotNull(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult?.Value as Dictionary<string, object?>;
            Assert.IsNotNull(data);

            var spellen = data?["Spellen"] as List<DamSpel>;
            Assert.IsNotNull(spellen);

            Assert.AreEqual(0, spellen.Count);
        }
        [Test]
        public async Task GetGameListOne()
        {
            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            await _controller.Create(newGame);
            var result = _controller.GetGameLijst();

            Assert.IsNotNull(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult?.Value as Dictionary<string, object?>;
            Assert.IsNotNull(data);

            var spellen = data?["Spellen"] as List<DamSpel>;
            Assert.IsNotNull(spellen);

            Assert.AreEqual(1, spellen.Count);
        }
        [Test]
        public async Task GetGameListMany()
        {
            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");
            await _controller.Create(newGame);
            await Task.Delay(300);

            newGame = new DamSpel(0, "Spel2", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");
            await _controller.Create(newGame);
            await Task.Delay(300);

            newGame = new DamSpel(0, "Spel3", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");
            await _controller.Create(newGame);
            await Task.Delay(300);

            var result = _controller.GetGameLijst();

            Assert.IsNotNull(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            var data = jsonResult?.Value as Dictionary<string, object?>;
            Assert.IsNotNull(data);

            var spellen = data?["Spellen"] as List<DamSpel>;
            Assert.IsNotNull(spellen);

            Assert.That(spellen.Count > 1);
        }

        [TearDown]
        public async Task TearDown()
        {
            ClearDatabase(_dbContext);
            await Task.Delay(500);
        }
        private void ClearDatabase(WebpageDBContext context)
        {
            context.ContactData.RemoveRange(context.ContactData);
            context.DamSpel.RemoveRange(context.DamSpel);
            context.DamBord.RemoveRange(context.DamBord);
            context.Speler.RemoveRange(context.Speler);
            context.SpelerStats.RemoveRange(context.SpelerStats);

            context.SaveChanges();
        }
    }
}


