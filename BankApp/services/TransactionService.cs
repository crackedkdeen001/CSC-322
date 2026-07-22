using BankApp.interfaces;
using BankApp.models;
using BankApp.exceptions;
using BankApp.repository;

namespace BankApp.services;

/// <summary>
/// The service layer of the transactions
/// </summary>
/// <param name="fileName"></param>
public class TransactionService(string fileName)
{
    private readonly TransactionRepository _transactionRepo =  new(fileName);

    /// <summary>
    /// Creates a new transaction<br/>
    /// Precondition: None<br/>
    /// Postcondition: The number of transactions in the database increases by 1
    /// </summary>
    /// <param name="accountId">The account ID of the account initiating the transaction</param>
    /// <param name="amount">Money deposited or withdrawed from the account</param>
    /// <param name="type">The type denoting if money was added (Credit) or removed (Debit) from the account</param>
    /// <param name="description">A short note on the transaction</param>
    public void CreateTransaction(int accountId, decimal amount, TransactionType type, string description)
    {
        var transaction = new Transaction(accountId, amount, type, description);
        _transactionRepo.Add(transaction);
    }

    /// <summary>
    /// Returns a list of transactions that an account has performed<br/>
    /// Precondition: None<br/>
    /// Postcondition: None
    /// </summary>
    /// <param name="accountId">The ID of the account whose transactions we want to see</param>
    /// <returns>A list of the transactions the account has performed</returns>
    public List<Transaction> GetAllTransactionsByAccount(int accountId)
    {
        return _transactionRepo.GetByAccountId(accountId);
    }

    /// <summary>
    /// Deletes all the transactions of an account
    /// </summary>
    /// <param name="accountId">The ID of the account</param>
    /// <returns>The number of deleted transactions</returns>
    public int DeleteTransactions(int accountId)
    {
        return _transactionRepo.DeleteByAccountId(accountId);
    }
}