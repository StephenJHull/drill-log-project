namespace WebApp.Models
{
    public class UserModel
    {
        public string Email { get; set; } = default!;

        public string Password { get; set; } = default!;

        public IEnumerable<string> Roles { get; set; } = default!;
    }
}
