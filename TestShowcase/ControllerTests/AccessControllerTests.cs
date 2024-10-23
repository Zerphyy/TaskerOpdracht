using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;
        }
        private void SeedDatabase(WebpageDBContext context)
        {
            context.Speler?.Add(new Gebruiker { Naam = "Zerphy", Email = "kevinspijker@kpnmail.nl", Wachtwoord = "$2b$10$JwvhN2r6HGRb4ZERckXxFusoNapW0o.gU9PWnnOfLpQUbRO2zyZzS", Rol = "Moderator" });
            context.SaveChanges();
        }
        [Test]
        public void RetrieveLoginViewUserNotLoggedIn()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = _controller.Login() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.ViewName);
        }
        [Test]
        public void RetrieveLoginUserLoggedIn()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "kevinspijker@kpnmail.nl"),
        new Claim(ClaimTypes.Name, "Kevin Spijker"),
        new Claim(ClaimTypes.Role, "Gebruiker")
            }, CookieAuthenticationDefaults.AuthenticationScheme));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            var result = _controller.Login() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }


        [Test]
        public void RetrieveRegisterView()
        {
            var result = _controller.Register() as ViewResult;

            Assert.AreEqual(null, result.ViewName);
        }
        
        [Test]
        public async Task RegisterNewUserFails()
        {
            var result = await _controller.Register(new RegisterModel() { Email = null, Password = "veryGoodPassw0rd!", Username = "RealPerson", VerifyPassword = "veryGoodPassw0rd!" }) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result?.ViewName);
        }
        [Test]
        public async Task RegisterNewUserWorks()
        {
            var result = await _controller.Register(new RegisterModel() { Email = "testuser@test.user", Password = "veryGoodPassw0rd!", Username = "RealPerson", VerifyPassword = "veryGoodPassw0rd!" }) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
            Assert.AreEqual("Access", result.ControllerName);
        }
        [Test]
        public async Task RetrieveValidateView()
        {
            var userMail = "kevinspijker@kpnmail.nl";
            var loginModel = new LoginModel { Email = userMail, StayLoggedIn = false };

            _controller.TempData["UserMail"] = userMail;
            _controller.TempData["LoginModel"] = JsonConvert.SerializeObject(loginModel);

            var result = await _controller.Validate() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.ViewName);
            Assert.AreEqual(userMail, result.ViewData["UserMail"]);
        }
        [Test]
        public void LoginUnknownUserFails()
        {
            var result = _controller.Login(new LoginModel() { Email = "Tester123@notamail.com", Password = "Tester12!@", StayLoggedIn = false, Username = "" }).Result;
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult, "Login should return ViewResult for unknown users.");
            Assert.AreEqual("User not found!", viewResult?.ViewData["ValidateMessage"]);
        }
        [Test]
        public void LoginKnownUserSucceeds()
        {

            var result = _controller.Login(new LoginModel() { Email = "kevinspijker@kpnmail.nl", Password = "Plusklas01!", StayLoggedIn = false, Username = "" }).Result;
            var viewResult = result as RedirectToActionResult;
            Assert.IsNotNull(viewResult, "Login should return ViewResult for unknown users.");
            Assert.AreEqual("Validate", viewResult?.ActionName);
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
