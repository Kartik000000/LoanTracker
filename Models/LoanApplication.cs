// Models/LoanApplication.cs
// A model (also called Entity) represents a real world object in our system
// INTERVIEW: "What is a model in ASP.NET Core?"
// Answer: A class that defines the shape/structure of our data

namespace LoanTracker.Models
{
    // This class maps directly to a table in our database
    // Each property = one column in the table
    public class LoanApplication
    {
        // Primary key - uniquely identifies each loan application
        // INTERVIEW: "What is the purpose of an Id field in a database?"
        public int Id { get; set; }

        // Applicant details
        // "required" keyword means this field cannot be null
        // INTERVIEW: "What is the difference between required and optional properties?"
        public required string ApplicantName { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        // Loan details
        // decimal is used for money - never use float or double for currency
        // INTERVIEW: "Why use decimal instead of float for money?"
        // Answer: float/double have precision issues with decimal numbers
        public decimal LoanAmount { get; set; }

        public required string LoanType { get; set; } 
        // Examples: "Home Loan", "Personal Loan", "Car Loan"

        // Application status - tracks where the application is in the process
        // INTERVIEW: "Why use a string for status instead of a boolean?"
        // Answer: Status can have multiple states, not just true/false
        public string Status { get; set; } = "Pending";
        // Default value is Pending when a new application is created
        // Other values: "Under Review", "Approved", "Rejected"

        // Timestamps - important for audit trails in financial applications
        // INTERVIEW: "Why do we store CreatedAt and UpdatedAt?"
        // Answer: Audit trail - know when records were created and modified
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // UTC time is used instead of local time
        // INTERVIEW: "Why UTC instead of local time?"
        // Answer: Consistent across time zones, no daylight saving issues

        public DateTime? UpdatedAt { get; set; }
        // "?" makes it nullable - it's null until the record is updated
        // INTERVIEW: "What is a nullable type in C#?"
        // Answer: A value type that can also hold null (DateTime? = DateTime or null)
    }
}