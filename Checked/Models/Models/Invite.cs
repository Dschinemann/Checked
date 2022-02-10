namespace Checked.Models.Models
{
    public class Invite
    {
        public int Id { get; set; }
        public Organization Organization { get; set; }
        public virtual int OrganizationId { get; set; }
        public string Email { get; set; }
    }
}
