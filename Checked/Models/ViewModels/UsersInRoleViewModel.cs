namespace Checked.Models.ViewModels
{
    public class UsersInRoleViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Role { get; set; }
    }
}
