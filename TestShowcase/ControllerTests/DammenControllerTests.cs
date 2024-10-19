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
            // Mock the IHubContext
            _mockHubContext = new Mock<IHubContext<GameHub>>();

            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<WebpageDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Give your test database a name
                .Options;

            _dbContext = new WebpageDBContext(options);

            // Optionally, seed data into the in-memory database if necessary
            SeedDatabase(_dbContext);

            // Initialize the controller with the mocked dependencies
            _controller = new DammenController(_mockHubContext.Object, _dbContext);
        }

        // Optional: A method to seed initial data into the in-memory database
        private void SeedDatabase(WebpageDBContext context)
        {
            context.Speler?.Add(new Gebruiker { Naam = "Zerphy", Email = "kevinspijker@kpnmail.nl", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password", BCrypt.Net.BCrypt.GenerateSalt()), Rol = "Moderator" });
            context.Speler?.Add(new Gebruiker { Naam = "Zerphy2", Email = "kevinspijker2@kpnmail.nl", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password1", BCrypt.Net.BCrypt.GenerateSalt()), Rol = "Moderator" });

            context.SaveChanges();
        }
        //basic functionality
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
            // Arrange: Create a new game model
            DamSpel newGame1 = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");
            DamSpel newGame2 = new DamSpel(0, "Spel2", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            // Act: Create the first game
            var firstResult = await _controller.Create(newGame1);

            // Assert: Ensure that the first game creation was successful
            var addedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.IsNotNull(addedGame, "The first game was not added to the database.");

            // Act: Try to create a second game immediately (within 1 second)
            var secondResult = await _controller.Create(newGame2);

            // Assert: Verify that the second game creation was denied
            Assert.IsInstanceOf<JsonResult>(secondResult, "The result should be a JsonResult.");
            var jsonResponse = secondResult as JsonResult;

            // Step 1: Convert the JsonResult Value into a JSON string
            string jsonString = JsonConvert.SerializeObject(jsonResponse.Value);

            // Step 2: Deserialize the JSON string into a dictionary or anonymous object
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

            // Assert: Check the content of the JSON response
            Assert.IsNotNull(jsonData, "The JsonResult data should not be null.");
            Assert.IsTrue(jsonData.ContainsKey("success"), "JsonResult should contain a 'success' key.");
            Assert.IsTrue(jsonData.ContainsKey("message"), "JsonResult should contain a 'message' key.");

            // Assert: Verify the success and message values
            Assert.IsFalse((bool)jsonData["success"], "The second game creation should have been blocked due to rate limiting.");
            Assert.AreEqual("You can only create one game per second.", jsonData["message"].ToString(), "The message for rate limiting is incorrect.");

            // Ensure that only the first game is in the database
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
            // Arrange: Create a new game
            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            // Act: Create the game (await for async method)
            await _controller.Create(newGame);

            // Check if the game was actually added to the database
            DamSpel addedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.IsNotNull(addedGame, "The game was not added to the database.");

            // Act: Delete the game (by its ID)
            var result = _controller.Delete(addedGame);  // Assuming your Delete method takes an ID

            // Assert: The delete action result should not be null
            Assert.IsNotNull(result);

            // Assert: Ensure the game was removed from the database
            DamSpel deletedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.Id == addedGame.Id);
            Assert.IsNull(deletedGame, "The game was not deleted from the database.");
        }
        [Test]
        public async Task ViewGameTest()
        {
            // Arrange: Mock ClaimsPrincipal and set a user identity
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "kevinspijker@kpnmail.nl"), // Mock the authenticated user's email
        new Claim(ClaimTypes.Name, "Kevin Spijker")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Arrange: Create a new game
            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            // Act: Create the game (await for async method)
            await _controller.Create(newGame);

            // Check if the game was actually added to the database
            DamSpel addedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.IsNotNull(addedGame, "The game was not added to the database.");

            // Act: View the game (by its ID)
            var result = _controller.Spel(addedGame.DamBordId);  // Assuming your Spel method takes an ID

            // Assert: The result should not be null and should return a view
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);

            // Assert: Check if the view data contains expected values
            var viewResult = result as ViewResult;
            Assert.AreEqual(JsonConvert.SerializeObject(new string[] { newGame.Creator, "" }), viewResult.ViewData["Spelers"]);
            Assert.AreEqual("kevinspijker@kpnmail.nl", viewResult.ViewData["Gebruiker"]);
        }

        [Test]
        public async Task UpdateBoardTest()
        {
            // Arrange: Create a new game
            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            // Act: Create the game (await for async method)
            await _controller.Create(newGame);

            // Check if the game was actually added to the database
            DamSpel addedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            Assert.IsNotNull(addedGame, "The game was not added to the database.");

            // Act: Delete the game (by its ID)
            var result = _controller.UpdateBoardData("1234567890", addedGame.Id.ToString(), addedGame.AanZet == addedGame.Creator ? addedGame.Deelnemer : addedGame.Creator);  // Assuming your Delete method takes an ID
            DamSpel updatedGame = _dbContext.DamSpel?.FirstOrDefault(d => d.SpelNaam == "Spel1");
            // Assert: The delete action result should not be null
            Assert.AreNotEqual(updatedGame.BordStand, "0101010110101010010101010000000000000000202020200202020220202020");
        }
        [Test]
        public async Task ProcessWinAndLossTest()
        {
            // Arrange: Create a new game
            DamSpel newGame = new DamSpel(0, "Spel1", null, "kevinspijker@kpnmail.nl", null, 0, false, null, "kevinspijker@kpnmail.nl");

            // Act: Create the game (await for async method)
            await _controller.Create(newGame);

            // Check if the game was actually added to the database
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

            // Act: Delete the game (by its ID)
            var result = _controller.ProcessWin(updatedGame.Id.ToString(), updatedGame.Deelnemer, spelers, updatedGame.Deelnemer);  // Assuming your Delete method takes an ID

            // Assert: The delete action result should not be null
            Assert.IsNotNull(result);
        }
        [TearDown]
        public async Task TearDown()
        {
            ClearDatabase(_dbContext);
            await Task.Delay(500);
        }
        private void ClearDatabase(WebpageDBContext context)
        {
            // Remove all entities from each DbSet
            context.ContactData.RemoveRange(context.ContactData);
            context.DamSpel.RemoveRange(context.DamSpel);
            context.DamBord.RemoveRange(context.DamBord);
            context.Speler.RemoveRange(context.Speler);
            context.SpelerStats.RemoveRange(context.SpelerStats);

            context.SaveChanges();
        }
    }
}


