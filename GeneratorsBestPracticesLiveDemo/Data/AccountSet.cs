using System.Collections;
using System.Collections.Generic;

namespace GeneratorsBestPracticesLiveDemo.Data
{
  public class AccountSet : IEnumerable<Account>
  {
    private HashSet<Account> myAccounts;

    public AccountSet(HashSet<Account> accounts) => myAccounts = accounts;

    public IEnumerator<Account> GetEnumerator() => myAccounts.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
      return $@"Accounts [ {string.Join(", ", myAccounts) } ]";
    }
  }
}