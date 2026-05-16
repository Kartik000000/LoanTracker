using LoanTracker.Data;
using Microsoft.EntityFrameworkCore;
// Program.cs - Entry point of the ASP.NET Core application
// In .NET 6+, this replaced the old Startup.cs + Program.cs two-file setup

// WebApplication.CreateBuilder sets up the app with default configuration
// This includes: appsettings.json, environment variables, logging, and DI container
var builder = WebApplication.CreateBuilder(args);

// AddOpenApi() registers OpenAPI/Swagger documentation generation
// INTERVIEW: "What is Swagger?" - It auto-generates API documentation
// so other developers can see and test your endpoints without reading code
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AddControllers() registers the MVC controller pipeline
// INTERVIEW: "What is the difference between AddControllers() vs AddMvc()?"
// AddControllers() = API only (no views, no pages) - what we use for REST APIs
// AddMvc() = full MVC with views (for web pages)
// AddRazorPages() = for Razor page apps
builder.Services.AddControllers();
// Register DbContext with SQL Server
// INTERVIEW: "What does AddDbContext do?"
// Answer: Registers AppDbContext in the DI container so controllers can use it
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Build() finalizes the dependency injection container
// and creates the WebApplication instance
// After this line you CANNOT add more services
var app = builder.Build();

// Middleware pipeline starts here
// INTERVIEW: "What is middleware in ASP.NET Core?"
// Middleware = components that handle requests/responses in a pipeline
// Order matters - each middleware runs in the order you add it

if (app.Environment.IsDevelopment())
{
    // Only show API docs in Development environment, not in Production
    // INTERVIEW: "How do you manage environments in .NET?"
    // Answer: appsettings.Development.json, ASPNETCORE_ENVIRONMENT variable
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirects HTTP requests to HTTPS for security
// INTERVIEW: "What is HTTPS redirection middleware?"
app.UseHttpsRedirection();

// Tells the app to use attribute-based routing on controllers
// Looks for [Route], [HttpGet], [HttpPost] attributes in controller files
// INTERVIEW: "What is the difference between conventional and attribute routing?"
app.MapControllers();

// Starts the web server and begins listening for requests
// This is a blocking call - app runs until you press Ctrl+C
app.Run();