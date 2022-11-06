using System.ComponentModel.DataAnnotations;
using static CommunityManager.Infrastructure.Data.Constants.UserConstants;

namespace CommunityManager.Core.Models
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(UsernameMaxLenght)]
        [MinLength(UsernameMinLenght)]
        public string UserName { get; set; } = null!;

        [Required]
        [MaxLength(EmailMaxLenght)]
        [MinLength(EmailMinLenght)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(PasswordMaxLenght)]
        [MinLength(PasswordMinLenght)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
