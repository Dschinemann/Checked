namespace Checked.Models.Models
{
    public class Invite
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Organization Organization { get; set; }
        public virtual string OrganizationId { get; set; }
        public string Email { get; set; }
    }
}
