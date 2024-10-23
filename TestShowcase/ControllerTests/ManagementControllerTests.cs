using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Linq;
using System;
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
            context.Speler?.Add(new Gebruiker{Naam = "User1", Email = "user1@example.com",Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password", BCrypt.Net.BCrypt.GenerateSalt()),Rol = "Gebruiker"});
            context.Speler?.Add(new Gebruiker{Naam = "Admin",Email = "admin@example.com",Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password", BCrypt.Net.BCrypt.GenerateSalt()),Rol = "Admin"});
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
        public void RetrieveIndexView()
        {
            var result = _controller.Index() as ViewResult;

            Assert.AreEqual(null, result?.ViewName);
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
