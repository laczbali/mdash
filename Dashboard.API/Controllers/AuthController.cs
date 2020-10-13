using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dashboard.API.Data;
using Dashboard.API.DTOs;
using Dashboard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dashboard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        // website/api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Username already exists");
            }
            if(userForRegisterDto.Username != Regex.Replace(userForRegisterDto.Username, @"\s+", "")) {
                return BadRequest("Your username can't have any whitespace characters in it!");
            }

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username,
                UserLevel = AccessLevel.normal,
                Created = DateTime.Now,
                LastActive = DateTime.Now
            };
            if(await _repo.IsUsersEmpty())
            {
                // If this is the first user in the DB, set him to admin
                userToCreate.UserLevel = AccessLevel.admin;
            }

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        // website/api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {
            // Try to log in user
            var userFromRepo = await _repo.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);
            if (userFromRepo == null)
            {
                // Unsuccessful login
                return Unauthorized();
            }

            // Create JWT content
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username),
                new Claim(ClaimTypes.Role, userFromRepo.UserLevel.ToString())
            };

            // Sign JWT
            var securityToken = _config.GetSection("AppSettings:Token").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityToken));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Build JWT
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            // Create JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return token
            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}