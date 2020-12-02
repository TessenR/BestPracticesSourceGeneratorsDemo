using System.Linq;
using System.Threading;
using GeneratorsBestPracticesLiveDemo.Data;

namespace GeneratorsBestPracticesLiveDemo.Services
{
  public class ClientService : IClientService
  {
    private readonly IAccountingService _accountingService;

    public ClientService(IAccountingService lengthCalculator)
    {
      _accountingService = lengthCalculator;
    }

    public decimal GetTotalAccountBalanceRemainder(Client client)
    {
      var accountsSet = _accountingService.GetClientAccounts(client);
      Thread.Sleep(200);
      return accountsSet.Sum(x => x.Balance);
    }
  }
}