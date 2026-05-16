// Services/TokenService.cs
// This service is responsible for generating JWT tokens
// INTERVIEW: "What is a service in ASP.NET Core?"
// Answer: A class that contains business logic, registered in DI container
// and injected wherever needed

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoanTracker.Models;
using LoanTracker.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace LoanTracker.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        // IConfiguration gives access to appsettings.json values
        // INTERVIEW: "What is IConfiguration?"
        // Answer: It provides access to app configuration like appsettings.json,
        // environment variables etc. via DI
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            // Claims = information we embed inside the token
            // INTERVIEW: "What are claims in JWT?"
            // Answer: Key-value pairs embedded in the token that describe the user
            // e.g. who they are, what role they have
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Secret key used to sign the token
            // INTERVIEW: "Why do we sign JWT tokens?"
            // Answer: To ensure the token hasn't been tampered with.
            // Server signs it, and verifies the signature on every request
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build the token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            // Serialize token to string — this is what we send to the client
            // Looks like: xxxxx.yyyyy.zzzzz
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}