using System;
using System.Collections.Generic;
using GeneratorsBestPracticesLiveDemo.Data;

namespace GeneratorsBestPracticesLiveDemo.Services
{
  public class AccountingService : IAccountingService
  {
    public AccountSet GetClientAccounts(Client client)
    {
      return client.Name switch
      {
        "Petya" => new AccountSet(new HashSet<Account>(new [] {new Account { Type = AccountType.Debit, Balance = 15000, Id = 2}, new Account { Type = AccountType.Credit, Balance = -20000, Id = 3}})),
        _ => throw new Exception($"{client} not found!")
      };
    }
  }
}