using System;
using GeneratorsBestPracticesLiveDemo.Data;
using GeneratorsBestPracticesLiveDemo.Services;
using NLog;
using NLog.Config;
using NLog.Targets;
using LoggingImplDefault;

namespace GeneratorsBestPracticesLiveDemo
{
  class Program
  {
    static void Main(string[] args)
    {
      var config = new LoggingConfiguration();
      var logconsole = new ConsoleTarget("logconsole");
      config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
      LogManager.Configuration = config;

      var accounting = new AccountingService().WithLogging();
      var clientService = new ClientService(accounting).WithLogging();

      Test(clientService);
    }

    private static void Test(IClientService clientService)
    {
      try
      {
        var client1 = new Client {Name = "Petya", Email = "petya@gmail.com"};
        var client2 = new Client {Name = "Vasya", Email = "vasya@gmail.com"};
        clientService.GetTotalAccountBalanceRemainder(client1);
        Console.WriteLine();
        clientService.GetTotalAccountBalanceRemainder(client2);
      }
      catch
      {
      }
    }
  }
}