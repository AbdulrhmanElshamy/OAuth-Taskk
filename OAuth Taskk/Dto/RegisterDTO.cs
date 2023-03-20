using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace OAuth_Taskk.Dto
{
    public class RegisterDTO
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }


    }
}
