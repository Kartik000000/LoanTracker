// Controllers/AuthController.cs
// Handles user registration and login
// INTERVIEW: "What is the purpose of an Auth Controller?"
// Answer: It handles authentication — registering new users and logging in existing ones

using LoanTracker.Data;
using LoanTracker.Interfaces;
using LoanTracker.Models;
using LoanTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
namespace LoanTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        // DI injects both AppDbContext and TokenService automatically
        // INTERVIEW: "How does DI know what to inject?"
        // Answer: It reads the constructor parameters and resolves them
        // from the registered services in Program.cs
        public AuthController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            // Check if username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == request.Username);

            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            // Hash the password before saving
            // INTERVIEW: "Why do we hash passwords?"
            // Answer: Never store plain text passwords. If DB is hacked,
            // hashed passwords are useless to attackers
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            // Find user by username
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Username == request.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            // Verify password
            if (user.PasswordHash != HashPassword(request.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            // Generate JWT token
            var token = _tokenService.GenerateToken(user);

            return Ok(new { token });
        }

        // Helper method to hash passwords using SHA256
        // INTERVIEW: "What hashing algorithm should you use for passwords?"
        // Answer: BCrypt or Argon2 in production. SHA256 is shown here for simplicity.
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    // Request models — these define what the client sends
    // INTERVIEW: "What is a DTO?"
    // Answer: Data Transfer Object — a class used to transfer data between
    // client and server, separate from the database model
    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}