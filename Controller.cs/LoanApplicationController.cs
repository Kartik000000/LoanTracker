// Controllers/LoanApplicationController.cs
using LoanTracker.Data;
using LoanTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoanApplicationController : ControllerBase
    {
        // Injecting AppDbContext via constructor injection
        // INTERVIEW: "What is constructor injection?"
        // Answer: Dependencies are passed through the constructor by the DI container
        // We no longer use a static list - we use the real database now
        private readonly AppDbContext _context;

        public LoanApplicationController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/loanapplication
        [HttpGet]
        public async Task<ActionResult<List<LoanApplication>>> GetAll()
        {
            // ToListAsync() fetches all records from LoanApplications table
            // INTERVIEW: "What is async/await in C#?"
            // Answer: Allows non-blocking operations - thread is free while waiting for DB
            var applications = await _context.LoanApplications.ToListAsync();
            return Ok(applications);
        }

        // GET api/loanapplication/1
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanApplication>> GetById(int id)
        {
            // FindAsync() finds a record by primary key
            // INTERVIEW: "What is the difference between Find and FirstOrDefault?"
            // Answer: Find checks cache first then DB, FirstOrDefault always queries DB
            var application = await _context.LoanApplications.FindAsync(id);

            if (application == null)
            {
                return NotFound($"Loan application with ID {id} not found");
            }

            return Ok(application);
        }

        // POST api/loanapplication
        [HttpPost]
        public async Task<ActionResult<LoanApplication>> Create([FromBody] LoanApplication application)
        {
            application.CreatedAt = DateTime.UtcNow;
            application.Status = "Pending";

            // Add to DbContext (tracks the new entity)
            // INTERVIEW: "What does _context.Add() do?"
            // Answer: Tells EF Core to track this entity and insert it on SaveChanges
            _context.LoanApplications.Add(application);

            // SaveChangesAsync() executes the INSERT SQL statement
            // INTERVIEW: "What is SaveChanges in EF Core?"
            // Answer: Commits all tracked changes to the database in one transaction
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = application.Id }, application);
        }

        // PUT api/loanapplication/1/status
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var application = await _context.LoanApplications.FindAsync(id);

            if (application == null)
            {
                return NotFound($"Loan application with ID {id} not found");
            }

            var validStatuses = new[] { "Pending", "Under Review", "Approved", "Rejected" };
            if (!validStatuses.Contains(status))
            {
                return BadRequest($"Invalid status. Valid values: {string.Join(", ", validStatuses)}");
            }

            application.Status = status;
            application.UpdatedAt = DateTime.UtcNow;

            // SaveChangesAsync() executes the UPDATE SQL statement
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}