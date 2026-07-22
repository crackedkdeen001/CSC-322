using BankApp.models;
using BankApp.services;

var transService = new TransactionService("transactions.json");
var accService = new AccountService("accounts.json");
