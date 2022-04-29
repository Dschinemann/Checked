using Checked.Models.Statics;

namespace Checked.Models.Models
{
    public class Task
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public TP_TaskStatus Status { get; set; }
        public virtual int StatusID { get; set; }
        public ApplicationUser User { get; set; }
        public virtual string UserId { get; set; }
    }
}
