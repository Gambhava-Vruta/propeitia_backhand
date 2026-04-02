//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Propertia.Models;

//[ApiController]
//[Route("api/[controller]")]
//public class UsersController : ControllerBase
//{
//    private readonly PropertiaContext _db;

//    public UsersController(PropertiaContext db)
//    {
//        _db = db;
//    }


//    [HttpGet]
//    public async Task<IActionResult> GetAll()
//    {
//        try
//        {
//            var users = await _db.Users
//                .Select(u => new 
//                {
//                    u.UserId,
//                    u.Name,
//                    u.Email,
//                    u.Phone,
//                    u.UserType,
//                    u.CreatedAt
//                })
//                .ToListAsync();

//            return Ok(users);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error getting users",
//                error = ex.Message
//            });
//        }
//    }


//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        try
//        {
//            var user = await _db.Users
//                .Where(u => u.UserId == id)
//                .Select(u => new
//                {
//                    u.UserId,
//                    u.Name,
//                    u.Email,
//                    u.Phone,
//                    u.UserType,
//                    u.CreatedAt
//                })
//                .FirstOrDefaultAsync();

//            if (user == null)
//                return NotFound(new { message = "User not found" });

//            return Ok(user);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error getting user",
//                error = ex.Message
//            });
//        }
//    }


//    [HttpPost]
//    public async Task<IActionResult> Create(User user)
//    {
//        try
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 6)
//                return BadRequest(new { message = "Password must be at least 6 characters" });

//            bool emailExists = await _db.Users.AnyAsync(u => u.Email == user.Email);
//            if (emailExists)
//                return BadRequest(new { message = "Email already exists" });

//            user.CreatedAt = DateTime.Now;

//            _db.Users.Add(user);
//            await _db.SaveChangesAsync();

//            return Created("", new
//            {
//                user.UserId,
//                user.Name,
//                user.Email
//            });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error creating user",
//                error = ex.Message
//            });
//        }
//    }


//    [HttpPut("{id}")]
//    public async Task<IActionResult> Update(int id, User user)
//    {
//        try
//        {
//            var existing = await _db.Users.FindAsync(id);

//            if (existing == null)
//                return NotFound(new { message = "User not found" });

//            existing.Name = user.Name;
//            existing.Email = user.Email;
//            existing.Phone = user.Phone;
//            existing.UserType = user.UserType;

//            if (!string.IsNullOrWhiteSpace(user.Password))
//            {
//                if (user.Password.Length < 6)
//                    return BadRequest(new { message = "Password must be at least 6 characters" });

//                existing.Password = user.Password;
//            }

//            await _db.SaveChangesAsync();

//            return Ok(new { message = "User updated successfully" });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error updating user",
//                error = ex.Message
//            });
//        }
//    }


//    [HttpDelete("{id}")]
//    public async Task<IActionResult> Delete(int id)
//    {
//        try
//        {
//            var user = await _db.Users.FindAsync(id);

//            if (user == null)
//                return NotFound(new { message = "User not found" });

//            _db.Users.Remove(user);
//            await _db.SaveChangesAsync();

//            return Ok(new { message = "User deleted successfully" });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                message = "Error deleting user",
//                error = ex.Message
//            });
//        }
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Propertia.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly PropertiaContext _db;
    private readonly IConfiguration _configuration;


    public UsersController(PropertiaContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;

    }
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.UserType.ToLower()),


    new Claim("UserId", user.UserId.ToString()),              
    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

        var expiryMinutes = Convert.ToDouble(jwtSettings["TokenExpiryMinutes"]);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    [Authorize(Roles = "admin,buyer,seller")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _db.Users
                .Select(u => new Userdto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Password=u.Password,
                    Phone = u.Phone,
                    UserType = u.UserType,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error getting users", error = ex.Message });
        }
    }

    [Authorize(Roles="admin,buyer,seller")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _db.Users
                .Where(u => u.UserId == id)
                .Select(u => new Userdto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    UserType = u.UserType,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error getting user", error = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(Userdto dto)
    {
        try
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            bool emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                return BadRequest(new { message = "Email already exists" });

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Password is required" });
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                UserType = dto.UserType,
                Password = dto.Password, 
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Created("", new Userdto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Password=user.Password,
                UserType = user.UserType,
                CreatedAt = user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating user", error = ex.Message });
        }
    }

    [Authorize(Roles = "admin,buyer,seller")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateUserDto dto)
    {
        var existing = await _db.Users.FindAsync(id);

        if (existing == null)
            return NotFound(new { message = "User not found" });

        existing.Name = dto.Name;
        existing.Email = dto.Email;
        existing.Phone = dto.Phone;
        existing.UserType = dto.UserType;

        await _db.SaveChangesAsync();

        return Ok(new { message = "User updated successfully" });
    }


    [Authorize(Roles ="admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found" });

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting user", error = ex.Message });
        }
    }











    [AllowAnonymous]
    [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
{
    try
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            return BadRequest(new { message = "Email and password are required" });

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });

        if (user.Password != loginRequest.Password)
            return Unauthorized(new { message = "Invalid email or password" });
         
         var token = GenerateJwtToken(user);


            return Ok(new { token, user = new { user.UserId, user.Name, user.Email, user.Phone, user.UserType, user.CreatedAt } });
            // Return user data without password
        //    return Ok(new Userdto
        //{
                
        //    UserId = user.UserId,
        //    Name = user.Name,
        //    Email = user.Email,
        //    Phone = user.Phone,
        //    UserType = user.UserType,
        //    CreatedAt = user.CreatedAt,
        //    Password = null // Don't send password back
        //});
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Error during login", error = ex.Message });
    }
}

    [AllowAnonymous]
    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["GoogleClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.Credential, settings);

            if (payload == null)
            {
                return Unauthorized(new { message = "Invalid Google token" });
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user == null)
            {
                user = new User
                {
                    Name = payload.Name,
                    Email = payload.Email,
                    Password = "GOOGLE_OAUTH_NO_PASSWORD",
                    UserType = !string.IsNullOrWhiteSpace(request.Role) ? request.Role : "buyer",
                    CreatedAt = DateTime.Now
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token, user = new { user.UserId, user.Name, user.Email, user.Phone, user.UserType, user.CreatedAt } });
        }
        catch (InvalidJwtException ex)
        {
            return Unauthorized(new { message = "Invalid Google token", error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error during Google login", error = ex.Message });
        }
    }

}
