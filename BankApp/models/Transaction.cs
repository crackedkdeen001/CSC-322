using System.Text.Json.Serialization;
using BankApp.interfaces;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BankApp.models;

/// <summary>
/// An object modelling a single movement of money into or out of an account
/// </summary>
public class Transaction(int accountId, decimal amount, TransactionType type, string description = "") : IHasID
{
    /// <summary>
    /// The unique identifier of the transaction.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The id of the account this transaction belongs to
    /// </summary>
    public int AccountId { get; set; } = accountId;

    /// <summary>
    /// How much money moved. Always positive - the direction is given by <see cref="TransactionType"/>
    /// </summary>
    public decimal Amount { get; set; } = amount;

    /// <summary>
    /// Whether the money came in (Credit) or went out (Debit)
    /// </summary>
    ///
    public TransactionType Type { get; set; } = type;

    /// <summary>
    /// The date and time the transaction occurred
    /// </summary>
    public DateTime Time { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The short note on what the transaction
    /// </summary>
    public string Description { get; set; } = description;

    public override string ToString() =>
        $"[{Id}] {Time:yyyy-MM-dd HH:mm:ss} {Type,-6} {Amount,12:N2}  {Description}";
}
