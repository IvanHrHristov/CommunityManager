using System.ComponentModel.DataAnnotations;
using static CommunityManager.Infrastructure.Data.Constants.UserConstants;

namespace CommunityManager.Core.Models
{
    public class EditViewModel
    {
        public string Id { get; set; } = null!;

        [Required]
        [MaxLength(UsernameMaxLenght)]
        [MinLength(UsernameMinLenght)]
        public string UserName { get; set; } = null!;

        [Required]
        [MaxLength(EmailMaxLenght)]
        [MinLength(EmailMinLenght)]
        public string Email { get; set; } = null!;

        [Required]
        [Range(AgeMinValue, AgeMaxValue)]
        public int Age { get; set; }
    }
}
