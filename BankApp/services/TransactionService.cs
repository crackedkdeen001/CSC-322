using BankApp.models;
using BankApp.repository;

namespace BankApp.services;

/// <summary>
/// The service layer of the transactions.
/// </summary>
/// <param name="fileName">The name of the JSON file the transactions are stored in</param>
public class TransactionService(string fileName)
{
    private readonly TransactionRepository _transactionRepo = new(fileName);

    /// <summary>
    /// requires: none<br/>
    /// modifies: the transaction database<br/>
    /// effects: records a new transaction of the given amount and type against the account
    /// </summary>
    /// <param name="accountId">The account ID the transaction belongs to</param>
    /// <param name="amount">Money deposited into or withdrawn from the account</param>
    /// <param name="type">Whether money was added (Credit) or removed (Debit)</param>
    /// <param name="description">A short note on the transaction</param>
    public void CreateTransaction(int accountId, decimal amount, TransactionType type, string description)
    {
        var transaction = new Transaction(accountId, amount, type, description);
        _transactionRepo.Add(transaction);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns the account's transactions oldest first, or an empty list if it has none
    /// </summary>
    /// <param name="accountId">The ID of the account whose transactions we want to see</param>
    /// <returns>A list of the transactions the account has performed, oldest first</returns>
    public List<Transaction> GetAllTransactionsByAccount(int accountId)
    {
        return _transactionRepo.GetByAccountId(accountId);
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: nothing<br/>
    /// effects: returns every transaction in the bank, or an empty list if there are none
    /// </summary>
    /// <returns>A list of every transaction in the bank</returns>
    public List<Transaction> GetAllTransactions()
    {
        return _transactionRepo.GetAll();
    }

    /// <summary>
    /// requires: none<br/>
    /// modifies: the transaction database<br/>
    /// effects: removes every transaction belonging to the account and returns how many were removed
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>The number of deleted transactions</returns>
    public int DeleteTransactions(int accountId)
    {
        return _transactionRepo.DeleteByAccountId(accountId);
    }
}