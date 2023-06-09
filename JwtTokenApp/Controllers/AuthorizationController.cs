﻿using JwtTokenApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtTokenApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : Controller
    {
        private static List<User> UserList = new List<User>();
        private readonly AppSettings applicationSettings;

        public AuthorizationController(IOptions<AppSettings> _applicationSettings)
        {
            applicationSettings = _applicationSettings.Value;
        }
        [HttpPost("[action]")]
        public IActionResult Login([FromBody] Login model)
        {
            var user = UserList.Where(x=>x.UserName== model.UserName).FirstOrDefault();
            if (user == null) 
            {
                return BadRequest("Kullanıcı adı yada şifre doğru değildir.");
            }
            var match = CheckPassword(model.Password, user);
            if (!match)
            {
                return BadRequest("Kullanıcı adı yada şifre doğru değildir.");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(applicationSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {new Claim("id",user.UserName), new Claim(ClaimTypes.Role, user.Role) }),
                Expires= DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encrypterToken = tokenHandler.WriteToken(token);
            return Ok(new {token = encrypterToken,username = user.UserName});   

        }
        [HttpPost("[action]")]
        public IActionResult Register([FromBody] Register model)
        {
            var user = new User { UserName = model.UserName, Role = model.Role, BirthDay = model.BirthDay };
            if (model.ConfirmPassword==model.Password)
            {
                using (HMACSHA512 hmac = new HMACSHA512())
                {
                    user.PasswordSalt = hmac.Key;
                    user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password));
                }
            }
            else
            {
                return BadRequest("Şifreniz ile Onay-Şifreniz uyumlu değildir.");
            }
            UserList.Add(user);
            return Ok(user);
        }

        private bool CheckPassword(string password,User user)
        {
            bool result;
            using(HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt)) 
            {
                var compute = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                result = compute.SequenceEqual(user.PasswordHash);
            }
            return result;
        }
    }
}
