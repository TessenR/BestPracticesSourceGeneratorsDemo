using GeneratorsBestPracticesLiveDemo.Data;

namespace GeneratorsBestPracticesLiveDemo.Services
{
  [Log]
  public interface IClientService
  {
    decimal GetTotalAccountBalanceRemainder(Client client);
  }
}