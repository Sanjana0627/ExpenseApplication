using ExpenseApplication.Core.Entities;
using ExpenseApplication.Core.Interfaces;
using ExpenseApplication.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace ExpenseApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        //constructor
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _unitOfWork = unitOfWork;
        }
        // creates a new user account with Identity
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                RoleId = request.RoleId,
                ManagerId = request.ManagerId
            };
            var result=await _userManager.CreateAsync(user,request.Password);
            if(!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok(new { message = "User registered successfully" });
        }
        // checks username/password and returns a JWT if they're valid
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user =await _userManager.FindByNameAsync(request.Username);
            if(user==null)
                return Unauthorized(new { message = "Invalid username or password" });
            var result=await _signInManager.CheckPasswordSignInAsync(user,request.Password,false);
            if(!result.Succeeded)
                return Unauthorized(new { message = "Invalid username or password" });
            var token = await GenerateJwtToken(user);
            return Ok(new { token });
        }
        // builds the signed JWT (with the user's id, name and role as claims)
        private async Task<string> GenerateJwtToken(User user)
        {
            var jwtSettings=_config.GetSection("Jwt");
            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId);
            string roleName = role?.RoleName ?? "Unknown";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"] ?? "30")),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

