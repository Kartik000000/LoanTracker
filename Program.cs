using LoanTracker.Data;
using LoanTracker.Interfaces;
using LoanTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
builder.Services.AddSwaggerGen(options =>
{
    // Add JWT authorization button to Swagger UI
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// AddControllers() registers the MVC controller pipeline
// INTERVIEW: "What is the difference between AddControllers() vs AddMvc()?"
// AddControllers() = API only (no views, no pages) - what we use for REST APIs
// AddMvc() = full MVC with views (for web pages)
// AddRazorPages() = for Razor page apps
builder.Services.AddControllers();
// Register TokenService in DI container
// AddScoped = one instance per HTTP request
// INTERVIEW: "What is the difference between AddScoped, AddSingleton, AddTransient?"
builder.Services.AddScoped<ITokenService,TokenService>();

// Configure JWT Authentication
// INTERVIEW: "How do you add JWT authentication in ASP.NET Core?"
// Answer: AddAuthentication with JwtBearer scheme, configure token validation parameters
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate the server that created the token
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

            // Validate the app that uses the token
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],

            // Validate token expiry
            ValidateLifetime = true,

            // Validate the secret key
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });
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
// INTERVIEW: "What is the order of authentication middleware?"
// Answer: UseAuthentication must come before UseAuthorization
// Authentication = who are you?
// Authorization = what are you allowed to do?
app.UseAuthentication();
app.UseAuthorization();

// Tells the app to use attribute-based routing on controllers
// Looks for [Route], [HttpGet], [HttpPost] attributes in controller files
// INTERVIEW: "What is the difference between conventional and attribute routing?"
app.MapControllers();

// Starts the web server and begins listening for requests
// This is a blocking call - app runs until you press Ctrl+C
app.Run();