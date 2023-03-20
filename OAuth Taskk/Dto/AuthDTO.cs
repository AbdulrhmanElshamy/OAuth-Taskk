namespace OAuth_Taskk.Dto
{
    public class AuthDTO
    {
        public bool IsAuthenticated { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public DateTime ExpireOn { get; set; }
    }
}
