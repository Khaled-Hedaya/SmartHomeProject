namespace SmartHomeProject.Models
{
    public class Room : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "Room";
        public string ImageUrl { get; set; }

        // Navigation properties
        public virtual ICollection<Item> Items { get; set; }
    }
}
