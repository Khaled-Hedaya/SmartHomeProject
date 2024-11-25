namespace SmartHomeProject.Models
{
    public class Complaint : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public Guid UserId { get; set; }
        public Guid ItemId { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Item Item { get; set; }
    }
}
