using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public List<string> Stocks { get; set; } = new List<string>();
    public List<string> Cryptos { get; set; } = new List<string>();
}