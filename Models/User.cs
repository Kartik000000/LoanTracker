namespace LoanTracker.Models
{
    public class User
    {
        public int Id { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }
        public required string PasswordHash { get; set; }

        public string Role { get; set; } = "User";
        // Roles: "User" = normal user, "Admin" = can approve/reject loans

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
