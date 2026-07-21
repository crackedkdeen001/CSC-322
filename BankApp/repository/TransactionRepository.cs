using BankApp.models;

namespace BankApp.repository;

/// <summary>
/// A repository used for interacting with the database of the transactions
/// </summary>
public class TransactionRepository(string fileName) : JsonRepository<Transaction>(fileName)
{
    /// <summary>
    /// Gets every transaction belonging to one account, oldest first<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Returns that account's transactions, or an empty list if it has none
    /// </summary>
    /// <param name="accountId">The id of the account whose transactions are wanted</param>
    /// <returns>That account's transactions, oldest first</returns>
    public List<Transaction> GetByAccountId(int accountId) =>
        GetAll()
            .Where(transaction => transaction.AccountId == accountId)
            .OrderBy(transaction => transaction.Time)
            .ToList();

    /// <summary>
    /// Deletes every transaction belonging to one account<br/>
    /// PreCondition: None<br/>
    /// PostCondition: No transaction in the database has the given account id
    /// </summary>
    /// <param name="accountId">The id of the account whose transactions should go</param>
    /// <returns>The number of transactions deleted</returns>
    public int DeleteByAccountId(int accountId)
    {
        var transactions = GetAll();
        int removed = transactions.RemoveAll(transaction => transaction.AccountId == accountId);

        if (removed > 0)
        {
            Save(transactions);
        }

        return removed;
    }
}
