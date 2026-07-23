using BankApp.exceptions;
using BankApp.interfaces;

namespace BankApp.models;

/// <summary>
/// An object modelling a bank account. The account owns its balance and the rules that protect it:
/// money can only be added or removed through <see cref="Deposit"/> and <see cref="Withdraw"/>, which
/// keep the balance positive and never let it go below zero.
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
    /// The amount of money the account currently holds. It can only be changed through
    /// <see cref="Deposit"/> and <see cref="Withdraw"/>, never set directly from outside.
    /// </summary>
    public decimal Balance { get; private set; } = balance;

    /// <summary>
    /// The date and time that the account was created
    /// </summary>
    public DateTime DateIssued { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// requires: amount is greater than 0<br/>
    /// modifies: this account's balance<br/>
    /// effects: increases the balance by amount; throws InvalidAmountException if amount is not positive
    /// </summary>
    /// <param name="amount">The amount of money to add to the account</param>
    /// <exception cref="InvalidAmountException">thrown if the amount is not greater than 0</exception>
    public void Deposit(decimal amount)
    {
        if (amount <= 0m) throw new InvalidAmountException("Amount must be greater than 0.");

        Balance += amount;
    }

    /// <summary>
    /// requires: amount is greater than 0 and no greater than the current balance<br/>
    /// modifies: this account's balance<br/>
    /// effects: decreases the balance by amount; throws InvalidAmountException if amount is not positive,
    ///          or InsufficientFundsException if amount is greater than the balance
    /// </summary>
    /// <param name="amount">The amount of money to take out of the account</param>
    /// <exception cref="InvalidAmountException">thrown if the amount is not greater than 0</exception>
    /// <exception cref="InsufficientFundsException">thrown if the amount is greater than the balance</exception>
    public void Withdraw(decimal amount)
    {
        if (amount <= 0m) throw new InvalidAmountException("Amount must be greater than 0.");
        if (amount > Balance) throw new InsufficientFundsException("Insufficient funds");

        Balance -= amount;
    }

    public override string ToString() => $"[{Id}] {Username} - {Balance:N2} (opened {DateIssued:yyyy-MM-dd})";
}
