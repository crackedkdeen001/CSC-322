using System.Text.Json.Serialization;
using BankApp.interfaces;

namespace BankApp.models;

/// <summary>
/// An object modelling the account class
/// </summary>
/// <param name="username">The name of the account owner</param>
/// <param name="balance">The opening balance of the account</param>
public class Account(string username, decimal balance) : IHasID
{
    
    /// <summary>
    /// The unique identifier of the account
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The username of the owner of the account
    /// </summary>
    public string Username { get; set; } = username;

    /// <summary>
    /// The amount of money an account currently has
    /// </summary>
    public decimal Balance { get; set; } = balance;

    /// <summary>
    /// The date and time that the account was created
    /// </summary>
    public DateTime DateIssued { get; set; } = DateTime.UtcNow;

    public override string ToString() => $"[{Id}] {Username} - {Balance:N2} (opened {DateIssued:yyyy-MM-dd})";
}
