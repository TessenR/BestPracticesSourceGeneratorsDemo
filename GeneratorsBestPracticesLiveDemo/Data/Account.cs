namespace GeneratorsBestPracticesLiveDemo.Data
{
  public class Account
  {
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public AccountType Type { get; set; }

    public override string ToString()
    {
      return $"Account {{ {nameof(Id)}: {Id}, {nameof(Type)}: {Type}, {nameof(Balance)}: {Balance} }}";
    }
  }
}