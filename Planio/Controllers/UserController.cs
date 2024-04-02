using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Planio.Models;
using Planio.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;

namespace Planio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly StudentsService _studentsService;
        private readonly TeachersService _teachersService;
        private readonly AdminService _adminService;
        private readonly UserService _userService;

        private readonly PasswordService _passwordService;

        private readonly IConfiguration _configuration;

        public UserController(PasswordService passwordService, IConfiguration configuration, TeachersService teachersService, StudentsService studentsService, UserService userService, AdminService adminService)
        {
            _passwordService = passwordService;
            _configuration = configuration;
            _teachersService = teachersService;
            _studentsService = studentsService;
            _userService = userService;
            _adminService = adminService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto userData)
        {
            if (userData == null)
            {
                return BadRequest("Sie haben das Passwort und den Benutzernamen vergessen. (╯°□°）╯︵ ┻━┻");
            }

            var user = await _userService.GetUserWithEmail(userData.Email);
            if (user != null)
            {
                Console.WriteLine(user);
                bool success = _passwordService.VerifyPassword(user.Password, userData.Password);
                if (success)
                {
                    string token = CreateToken(user);
                    return Ok(token);
                }
            }

            return BadRequest("Benutzername oder Passwort falsch (╯°□°）╯︵ ┻━┻");
        }

        [HttpGet("GetRole")]
        [Authorize(Roles = "admin, teacher, student")]
        public async Task<IActionResult> GetRole()
        {
            var student = await _studentsService.GetById();
            if (student != null){ return Ok("student"); }
            var teacher = await _teachersService.GetById();
            if (teacher != null){ return Ok("teacher");}
            var admin = await _adminService.GetById();
            if (admin != null) { return Ok("admin"); }
            else
            {
                return NotFound("Der Benutzer wurde nicht gefunden");
            }
        }

        private string CreateToken(UserModel user)
        {
            string issuer = _configuration.GetSection("Jwt:Issuer").Value!;
            string audience = _configuration.GetSection("Jwt:Audience").Value!;

            List<Claim> claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Email),
                    new Claim(ClaimTypes.Role,  (user.Role))
            };

            string base64Key = _configuration.GetSection("Jwt:Key").Value!;
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Convert.FromBase64String(base64Key));

            SigningCredentials credentials = new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha512Signature);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
