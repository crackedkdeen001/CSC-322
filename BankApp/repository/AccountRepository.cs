using BankApp.models;

namespace BankApp.repository;

/// <summary>
/// A repository used for interacting with the database of the accounts
/// </summary>
public class AccountRepository(string fileName) : JsonRepository<Account>(fileName)
{
    /// <summary>
    /// Gets an account using the account username<br/>
    /// PreCondition: None<br/>
    /// PostCondition: Returns the account object if an account has that username, null if none does
    /// </summary>
    /// <param name="name">The username to look for</param>
    /// <returns>The account object if it exists, null if not</returns>
    public Account? GetByName(string name) => GetAll().FirstOrDefault(account => account.Username == name);
}
