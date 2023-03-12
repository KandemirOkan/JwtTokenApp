using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace JwtTokenApp.Models
{
    public class User
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string BirthDay { get; set; }
        public string Token { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
