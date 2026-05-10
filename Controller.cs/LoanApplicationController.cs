// Controllers/LoanApplicationController.cs
// A Controller handles incoming HTTP requests and returns responses
// INTERVIEW: "What is a Controller in ASP.NET Core?"
// Answer: A class that groups related API endpoints together

using LoanTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoanTracker.Controllers
{
    // [ApiController] enables automatic model validation and better error responses
    // INTERVIEW: "What does [ApiController] attribute do?"
    // Answer: Enables automatic 400 Bad Request on invalid input,
    // automatic binding of request body, and better error responses
    [ApiController]

    // [Route] defines the base URL for all endpoints in this controller
    // "api/[controller]" becomes "api/loanapplication" automatically
    // INTERVIEW: "What is attribute routing?"
    // Answer: Defining routes directly on controllers/actions using attributes
    [Route("api/[controller]")]
    public class LoanApplicationController : ControllerBase
    {
        // Temporary in-memory list to store applications
        // We will replace this with a real database later
        // INTERVIEW: "What is the difference between static and instance variables?"
        // Answer: static = shared across all requests, instance = per request
        private static List<LoanApplication> _applications = new List<LoanApplication>();
        private static int _nextId = 1;

        // GET api/loanapplication
        // Returns all loan applications
        // INTERVIEW: "What is the difference between GET and POST?"
        // GET = retrieve data, no body, safe and idempotent
        // POST = send data to create something new
        [HttpGet]
        public ActionResult<List<LoanApplication>> GetAll()
        {
            // ActionResult allows us to return both data AND HTTP status codes
            // INTERVIEW: "What is ActionResult?"
            // Answer: A type that can return any HTTP response (200, 404, 400 etc)
            return Ok(_applications);
            // Ok() = HTTP 200 with the data
        }

        // GET api/loanapplication/1
        // Returns a single application by Id
        [HttpGet("{id}")]
        public ActionResult<LoanApplication> GetById(int id)
        {
            // LINQ - Language Integrated Query
            // INTERVIEW: "What is LINQ?"
            // Answer: A way to query collections using C# syntax
            var application = _applications.FirstOrDefault(x => x.Id == id);

            if (application == null)
            {
                // Returns HTTP 404 Not Found
                return NotFound($"Loan application with ID {id} not found");
            }

            return Ok(application);
        }

        // POST api/loanapplication
        // Creates a new loan application
        [HttpPost]
        public ActionResult<LoanApplication> Create([FromBody] LoanApplication application)
        {
            // [FromBody] tells ASP.NET to read the data from the request body (JSON)
            // INTERVIEW: "What is [FromBody]?"
            // Answer: Tells the model binder to get data from the HTTP request body

            application.Id = _nextId++;
            application.CreatedAt = DateTime.UtcNow;
            application.Status = "Pending";

            _applications.Add(application);

            // CreatedAtAction returns HTTP 201 Created with the location of new resource
            // INTERVIEW: "Why return 201 instead of 200 for POST?"
            // Answer: 201 = resource was created, 200 = request succeeded
            // REST convention says POST should return 201
            return CreatedAtAction(nameof(GetById), new { id = application.Id }, application);
        }

        // PUT api/loanapplication/1/status
        // Updates the status of an application
        [HttpPut("{id}/status")]
        public ActionResult UpdateStatus(int id, [FromBody] string status)
        {
            var application = _applications.FirstOrDefault(x => x.Id == id);

            if (application == null)
            {
                return NotFound($"Loan application with ID {id} not found");
            }

            // Validate status value
            var validStatuses = new[] { "Pending", "Under Review", "Approved", "Rejected" };
            if (!validStatuses.Contains(status))
            {
                // Returns HTTP 400 Bad Request
                // INTERVIEW: "What is the difference between 400 and 404?"
                // 400 = bad request (client sent wrong data)
                // 404 = not found (resource doesn't exist)
                return BadRequest($"Invalid status. Valid values: {string.Join(", ", validStatuses)}");
            }

            application.Status = status;
            application.UpdatedAt = DateTime.UtcNow;

            // NoContent = HTTP 204 - success but nothing to return
            // INTERVIEW: "When do you return 204 vs 200?"
            // 204 = operation succeeded but no data to send back
            return NoContent();
        }
    }
}