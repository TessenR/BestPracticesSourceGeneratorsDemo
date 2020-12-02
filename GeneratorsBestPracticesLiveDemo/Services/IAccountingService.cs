using GeneratorsBestPracticesLiveDemo.Data;

namespace GeneratorsBestPracticesLiveDemo.Services
{
  [Log]
  public interface IAccountingService
  {
    AccountSet GetClientAccounts(Client client);
  }
}