using BankApp;
using BankApp.Models;
using BankApp.services;
using BankApp.utils;

AccountService accountService = new("accounts.json");
try
{
    int accountId = 1;
    
    accountService.GetAccountDetails(accountId);

    
    accountService.Deposit(1, 20.34m);
    var ( prev, current) =accountService.Withdraw(1, -20m);
    Console.WriteLine("prev: : "+ prev + " current: " + current);
}
catch (Exception e)
{
    Console.WriteLine(e);
}