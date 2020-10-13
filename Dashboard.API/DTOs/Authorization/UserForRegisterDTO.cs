using System.ComponentModel.DataAnnotations;

namespace Dashboard.API.DTOs
{
    /// <summary>
    /// Parse and validate HTML login request
    /// </summary>
    public class UserForRegisterDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 4, ErrorMessage = "You must choose a password thats between 4 and 32 characters")]
        public string Password { get; set; }
    }
}