using BankApp.interfaces;

namespace BankApp.Models;

public class Transaction: IHasID
{
    public int Id { get;  set; }
    public double Amount { get; set; }
    public TransactionType TransactionType { get; set; }

}