namespace BankApp.exceptions;

/// <summary>
/// An exception class thrown when the amount specified is invalid
/// </summary>
public class InvalidAmountException : Exception
{
    public InvalidAmountException()
    {
    }

    public InvalidAmountException(string message) : base(message)
    {
    }

    public InvalidAmountException(string message, Exception inner) : base(message, inner)
    {
    }
}