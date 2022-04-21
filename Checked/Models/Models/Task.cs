using Checked.Models.Statics;

namespace Checked.Models.Models
{
    public class Task
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public TP_TaskStatus Status { get; set; }
    }
}
