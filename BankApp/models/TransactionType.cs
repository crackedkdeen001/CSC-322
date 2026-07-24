namespace BankApp.models;

/// <summary>
/// Represents the type of transaction in the system.
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// A transaction where money is transferred out of the account
    /// </summary>
    Debit,
    /// <summary>
    /// A transaction where money is transferred into the account
    /// </summary>
    Credit
}