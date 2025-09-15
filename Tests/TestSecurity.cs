using NUnit.Framework;
using SafeVault.Controllers;
using SafeVault.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace SafeVault.Tests
{
    [TestFixture]
    public class TestSecurity
    {
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "SecurityTestDB")
                .Options;

            var context = new AppDbContext(options);
            _controller = new UserController(context);
        }

        [Test]
        public void TestSQLInjectionAttempt()
        {
            var result = _controller.AddUser("' OR '1'='1", "test@example.com", "password") as IActionResult;
            Assert.IsInstanceOf<OkObjectResult>(result); 
        }

        [Test]
        public void TestXSSAttempt()
        {
            var result = _controller.AddUser("<script>alert('XSS')</script>", "xss@example.com", "password") as IActionResult;
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }
}
