using System.ComponentModel.DataAnnotations;
using static CommunityManager.Infrastructure.Data.Constants.UserConstants;

namespace CommunityManager.Core.Models.User
{
    public class ChangePasswordViewModel
    {
        [Required]
        [MaxLength(PasswordMaxLenght)]
        [MinLength(PasswordMinLenght)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = null!;

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
