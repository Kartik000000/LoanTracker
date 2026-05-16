// Data/AppDbContext.cs
// DbContext is the bridge between your C# code and the database
// INTERVIEW: "What is DbContext in EF Core?"
// Answer: It represents a session with the database - used to query and save data

using Microsoft.EntityFrameworkCore;
using LoanTracker.Models;

namespace LoanTracker.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor - receives options (like connection string) via DI
        // INTERVIEW: "What is constructor injection?"
        // Answer: Dependencies are passed through the constructor by the DI container
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        // DbSet = represents a table in your database
        // INTERVIEW: "What is DbSet in EF Core?"
        // Answer: DbSet<T> maps to a database table - used to query and save records
        public DbSet<LoanApplication> LoanApplications { get; set; }
        // Configures the database schema
        // INTERVIEW: "What is OnModelCreating in EF Core?"
        // Answer: It's used to configure entity mappings, relationships, and column types
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        // Specify decimal precision for money fields
        // 18 = total digits, 2 = decimal places (e.g. 9999999999999999.99)
        // INTERVIEW: "Why specify precision for decimal in SQL Server?"
        // Answer: Prevents silent truncation of values, important for financial data
        modelBuilder.Entity<LoanApplication>()
        .Property(x => x.LoanAmount)
        .HasColumnType("decimal(18,2)");
        }
  }
    
}