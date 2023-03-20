using Microsoft.Build.Framework;

namespace OAuth_Taskk.Dto
{
    public class TokenRequestDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
