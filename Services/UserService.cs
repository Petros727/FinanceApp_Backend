using System.Security.Cryptography;
using System.Text;
using FinancialApp.Data;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> CreateUser(string username, string password)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Username == username))
            return false; 
        var salt = GenerateSalt();
        var passwordHash = HashPassword(password, salt);
        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            Salt = salt
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SaveUserPreferences(string username, List<string> stocks, List<string> cryptos)
{
    var user = await GetUserByUsername(username);
    if (user == null) return false;

    user.Stocks = stocks;
    user.Cryptos = cryptos;

    _dbContext.Users.Update(user);
    await _dbContext.SaveChangesAsync();
    return true;
}

public async Task<List<string>?> GetUserPreferences(string username, string preferenceType)
{
    var user = await GetUserByUsername(username);
    if (user == null) return null;

    if (preferenceType == "stocks")
    {
        return user.Stocks; 
    } 
    else if (preferenceType == "cryptos")
    {
        return user.Cryptos;
    }
    else 
    {
        return null;
    }
}

    public bool ValidatePassword(string password, string storedHash, string storedSalt)
    {
        var hash = HashPassword(password, storedSalt);
        return hash == storedHash;
    }

    private string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    private string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var saltedPassword = password + salt;
            var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
            var hashBytes = sha256.ComputeHash(saltedPasswordBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}