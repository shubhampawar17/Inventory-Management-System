using IMS.Contracts;
using IMS.Repository;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AdminUserRepository _adminUserRepository;

    public AuthController(AdminUserRepository adminUserRepository)
    {
        _adminUserRepository = adminUserRepository;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var username = request.Username?.Trim() ?? string.Empty;
        var password = request.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest(new LoginResponse(false, "Username and password are required.", string.Empty));
        }

        var user = await _adminUserRepository.ValidateCredentialsAsync(username, password);
        if (user == null)
        {
            return Unauthorized(new LoginResponse(false, "Invalid username or password.", string.Empty));
        }

        return Ok(new LoginResponse(true, "Login successful.", user.Username));
    }
}
