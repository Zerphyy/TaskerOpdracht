using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestShowcase.ControllerTests
{
    [TestFixture]
    internal class ManagementControllerTests
    {
        private ManagementController _controller;
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

            _controller = new ManagementController(_mockHubContext.Object, _dbContext);
        }
        private void SeedDatabase(WebpageDBContext context)
        {
            context.Speler?.Add(new Gebruiker { Naam = "Zerphy", Email = "kevinspijker@kpnmail.nl", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password", BCrypt.Net.BCrypt.GenerateSalt()), Rol = "Moderator" });
            context.SaveChanges();
        }

        [Test]
        public void RetrieveIndexView()
        {
            var result = _controller.Index();

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
            context.ContactData.RemoveRange(context.ContactData);
            context.DamSpel.RemoveRange(context.DamSpel);
            context.DamBord.RemoveRange(context.DamBord);
            context.Speler.RemoveRange(context.Speler);
            context.SpelerStats.RemoveRange(context.SpelerStats);

            context.SaveChanges();
        }
    }
}
