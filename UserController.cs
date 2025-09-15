using Microsoft.AspNetCore.Mvc;
using SafeVault.Data;
using SafeVault.Models;
using SafeVault.Utils;
using Microsoft.EntityFrameworkCore;

namespace SafeVault.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public IActionResult AddUser([FromForm] string username, [FromForm] string email, [FromForm] string password)
        {
            // Sanitize input to prevent XSS
            string safeUsername = InputSanitizer.Sanitize(username);
            string safeEmail = InputSanitizer.Sanitize(email);

            if (_context.Users.Any(u => u.Username == safeUsername))
                return BadRequest("Username already exists.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = safeUsername,
                Email = safeEmail,
                PasswordHash = passwordHash,
                Role = "user"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User added successfully.");
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.UserID == id);
            if (user == null)
                return NotFound("User not found.");

            return Ok(new { user.UserID, user.Username, user.Email, user.Role });
        }
    }
}
