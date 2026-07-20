using BankApp.interfaces;

namespace BankApp.Models;

/// <summary>
/// An object modelling a single movement of money into or out of an account
/// </summary>
public class Transaction : IHasID
{
    /// <summary>
    /// The unique identifier of the transaction.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The id of the account this transaction belongs to
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// How much money moved. Always positive - the direction is given by <see cref="TransactionType"/>
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Whether the money came in (Credit) or went out (Debit)
    /// </summary>
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// The date and time the transaction occurred
    /// </summary>
    public DateTime Time { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The short note on what the transaction
    /// </summary>
    public string Description { get; set; } = "";

    public override string ToString() =>
        $"[{Id}] {Time:yyyy-MM-dd HH:mm:ss} {TransactionType,-6} {Amount,12:N2}  {Description}";
}
