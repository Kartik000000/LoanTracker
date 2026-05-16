// Interfaces/ITokenService.cs
// This is the CONTRACT — defines what TokenService must do
// INTERVIEW: "What is an interface in C#?"
// Answer: A contract that defines what methods a class must implement
// without defining HOW they are implemented

using LoanTracker.Models;

namespace LoanTracker.Interfaces
{
    public interface ITokenService
    {
        // Any class implementing this interface MUST have this method
        // INTERVIEW: "What is the difference between interface and abstract class?"
        // Answer: Interface = pure contract, no implementation
        // Abstract class = partial implementation, can have some methods defined
        string GenerateToken(User user);
    }
}