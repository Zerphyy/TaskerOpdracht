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
    internal class AccessControllerTests
    {
        private AccessController _controller;
        private WebpageDBContext _dbContext;

        [SetUp]
        public void SetUp()
        {

            var options = new DbContextOptionsBuilder<WebpageDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new WebpageDBContext(options);

            SeedDatabase(_dbContext);

            _controller = new AccessController(_dbContext);
        }
        private void SeedDatabase(WebpageDBContext context)
        {
            context.Speler?.Add(new Gebruiker { Naam = "Zerphy", Email = "kevinspijker@kpnmail.nl", Wachtwoord = BCrypt.Net.BCrypt.HashPassword("password", BCrypt.Net.BCrypt.GenerateSalt()), Rol = "Moderator" });
            context.SaveChanges();
        }
        [Test]
        public void RetrieveLoginView()
        {
            var result = _controller.Login();

            Assert.IsNotNull(result);
        }
        [Test]
        public void RetrieveRegisterView()
        {
            var result = _controller.Register();

            Assert.IsNotNull(result);
        }
        [Test]
        public void RetrieveValidateView()
        {
            var result = _controller.Validate();

            Assert.IsNotNull(result);
        }
        [Test]
        public void LoginUnknownUserFails()
        {
            var result = _controller.Login(new LoginModel() { Email = "Tester123@notamail.com", Password = "Tester12!@", StayLoggedIn = false, Username = ""});

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
