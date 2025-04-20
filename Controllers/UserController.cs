using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Model;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly HomeDB _context;

    public UserController(HomeDB context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        if (await _context.Users.AnyAsync(x => x.Email == user.Email))
            return BadRequest("Email already registered");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "User registered successfully" });

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto login)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == login.Email && x.Password == login.Password);

        if (user == null)
            return Unauthorized("Invalid email or password");

        return Ok(new { message = "Login successful" });
    }

    // Get All Users
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    // Get User by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    // Update
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;
        user.Password = updatedUser.Password;
        user.MobileNumber = updatedUser.MobileNumber;

        await _context.SaveChangesAsync();
        return Ok(new { message = "User updated" });
    }

    // Delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "User deleted" });

    }

    [HttpPost("admin")]
    public async Task<IActionResult> adminLogin([FromBody] UserAdmin adminuser)
    {
        var user = await _context.UsersAdmin
            .FirstOrDefaultAsync(x => x.email == adminuser.email && x.password == adminuser.password);

        if (user == null)
            return NotFound();

        return Ok(new { message = "Login successful" });


    }
}
