using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }
    
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    var success = await _userService.CreateUser(request.Username, request.Password);
    if (!success)
        return BadRequest("Username already exists");

    return Ok("User registered successfully");
}

    [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    var user = await _userService.GetUserByUsername(request.Username);   

    if (user == null || !_userService.ValidatePassword(request.Password, user.PasswordHash, user.Salt))
        return Unauthorized("Invalid username or password");

    var userStocks = await _userService.GetUserPreferences(request.Username, "stocks");
    var userCryptos = await _userService.GetUserPreferences(request.Username, "cryptos");

    return Ok(new { message = "Login successful", stocks = userStocks, cryptos = userCryptos }); 
}

[HttpPost("savePreferences")]
public async Task<IActionResult> SavePreferences([FromBody] UserPreferencesRequest request)
{
    var success = await _userService.SaveUserPreferences(request.Username, request.Stocks, request.Cryptos);
    if (!success)
        return BadRequest("Failed to save preferences");

    return Ok("Preferences saved successfully");
}


}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserPreferencesRequest
{
    public string Username { get; set; } = string.Empty;
    public List<string> Stocks { get; set; } = new List<string>();
    public List<string> Cryptos { get; set; } = new List<string>();
}