using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestShowcase.ControllerTests
{
    [TestFixture]
    internal class ManagementControllerTests
    {
        private ManagementController _controller;
        private Mock<IHubContext<GameHub>> _mockHubContext;
        private Mock<IHubClients> _mockClients;
        private Mock<IClientProxy> _mockClientProxy;
        private WebpageDBContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _mockHubContext = new Mock<IHubContext<GameHub>>();
            _mockClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockHubContext.Setup(hub => hub.Clients).Returns(_mockClients.Object);

            var options = new DbContextOptionsBuilder<WebpageDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new WebpageDBContext(options);

            SeedDatabase(_dbContext);

            _controller = new ManagementController(_mockHubContext.Object, _dbContext);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "kevinspijker@kpnmail.nl"),
                new Claim(ClaimTypes.Name, "Kevin Spijker"),
                new Claim(ClaimTypes.Role, "Moderator")
            }, CookieAuthenticationDefaults.AuthenticationScheme));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            Mock<ISession> sessionMock = new Mock<ISession>();
            _controller.HttpContext.Session = sessionMock.Object;
            SetUpUserConnections();
        }
        private void SeedDatabase(WebpageDBContext context)
        {
            context.Speler?.Add(new Gebruiker { Naam = "Zerphy", Email = "kevinspijker@kpnmail.nl", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password", BCrypt.Net.BCrypt.GenerateSalt()), Rol = "Moderator" });
            context.Speler?.Add(new Gebruiker { Naam = "User1", Email = "user1@example.com", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password", BCrypt.Net.BCrypt.GenerateSalt()), Rol = "Gebruiker" });
            context.Speler?.Add(new Gebruiker { Naam = "Admin", Email = "admin@example.com", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password", BCrypt.Net.BCrypt.GenerateSalt()), Rol = "Admin" });
            context.SaveChanges();
        }
        private void SetUpUserConnections()
        {
            _mockClients.Setup(clients => clients.User(It.IsAny<string>())).Returns(_mockClientProxy.Object);
        }

        [Test]
        public async Task RemoveUserOnExistingUserWorks()
        {
            var userIdToRemove = "user1@example.com";

            var result = await _controller.RemoveUser(userIdToRemove) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsTrue((bool)result.Value?.GetType().GetProperty("success")?.GetValue(result.Value, null));

            var removedUser = await _dbContext.Speler.FindAsync(userIdToRemove);
            Assert.IsNull(removedUser);
            _mockClients.Verify(clients => clients.User(userIdToRemove), Times.Once);
            _mockHubContext.Verify(hub => hub.Clients.User(userIdToRemove), Times.Once);
        }
        [Test]
        public async Task RemoveUserOnNonExistingUserFails()
        {
            var userIdToRemove = "user2@example.com";

            var result = await _controller.RemoveUser(userIdToRemove) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(!(bool)result.Value?.GetType().GetProperty("success")?.GetValue(result.Value, null));

            var user = await _dbContext.Speler.FindAsync(userIdToRemove);
            Assert.IsNull(user);
        }
        [Test]
        public async Task PromoteUserOnExistingUserWorks()
        {
            var userIdToPromote = "user1@example.com";
            var result = await _controller.PromoteUser(userIdToPromote) as JsonResult;
            Assert.IsNotNull(result);
            Assert.IsTrue((bool)result.Value?.GetType().GetProperty("success")?.GetValue(result.Value, null));
            Assert.IsTrue(_dbContext.Speler?.FirstOrDefault(s => s.Email == userIdToPromote).Rol == "Moderator");
            _mockClients.Verify(clients => clients.User(userIdToPromote), Times.Once);
            _mockHubContext.Verify(hub => hub.Clients.User(userIdToPromote), Times.Once);
        }
        [Test]
        public async Task PromoteUserOnNonExistingUserFails()
        {
            var userIdToPromote = "user10000000@example.com";
            var result = await _controller.PromoteUser(userIdToPromote) as JsonResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(!(bool)result.Value?.GetType().GetProperty("success")?.GetValue(result.Value, null));
            Assert.IsTrue(_dbContext.Speler?.FirstOrDefault(s => s.Email == userIdToPromote) == null);
        }
        [Test]
        public void RetrieveIndexViewAsModeratorOrAdmin()
        {
            var result = _controller.Index() as ViewResult;

            Assert.AreEqual(null, result?.ViewName);
        }
        [Test]
        public void RetrieveIndexViewAsUser()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "user1@example.com"),
        new Claim(ClaimTypes.Name, "User1"),
        new Claim(ClaimTypes.Role, "Gebruiker")
            }, CookieAuthenticationDefaults.AuthenticationScheme));

            var context = new DefaultHttpContext
            {
                User = user,
                Session = new Mock<ISession>().Object
            };

            context.Request.Headers["Referer"] = "http://example.com/previous-page";

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            Assert.IsTrue(_dbContext.Speler?.FirstOrDefault(s => s.Email == "user1@example.com").Rol == "Gebruiker");

            var result = _controller.Index() as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("http://example.com/previous-page", result.Url);
        }
        [Test]
        public void GetUserLijstReturnsList()
        {
            var result = _controller.GetUserLijst() as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);

            Assert.IsTrue(result.Value is IEnumerable);

            var list = result.Value as IEnumerable<object>;
            Assert.IsNotNull(list);

            Assert.IsTrue(list.All(item => item is Gebruiker));
        }

        [TearDown]
        public async Task TearDown()
        {
            ClearDatabase(_dbContext);
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
