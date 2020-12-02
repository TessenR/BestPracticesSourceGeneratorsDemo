namespace GeneratorsBestPracticesLiveDemo.Data
{
  public class Client
  {
    public string Name { get; set; }
    public string Email { get; set; }

    public override string ToString()
    {
      return $"Client {{ {nameof(Name)}: {Name}, {nameof(Email)}: {Email} }}";
    }
  }
}