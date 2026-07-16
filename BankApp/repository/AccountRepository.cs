using BankApp.interfaces;
using BankApp.Models;
using BankApp.utils;

namespace BankApp.repository;

/// <summary>
/// A 
/// </summary>
/// <param name="fileName"></param>
public class AccountRepository(string fileName): IRepository<Account>
{
    private readonly JSONFileHandler<Account> _fileHandler = new(fileName);

    public List<Account> GetAll()
    {
        var accounts = _fileHandler.LoadItems();
        return accounts;
    }

    public Account? GetById(int id)
    {
        var accounts = GetAll();
        foreach (var account in accounts)
        {
            if (account.Id == id)
            {
                return account;
            }
        }

        return null;
    }

    public void Add(Account account)
    {
        var accounts = GetAll();
        accounts.Add(account);
        
        _fileHandler.SaveItems(accounts);
    }

    public Account? Delete(int id)
    {
        var accounts = GetAll();
        Account? deleted = GetById(id);
        
        if (deleted is not null)
        {
            accounts.Remove(deleted);
            _fileHandler.SaveItems(accounts);
        }
        
        return deleted;
    }
}