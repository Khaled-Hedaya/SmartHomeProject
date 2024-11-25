namespace SmartHomeProject.Models
{
    public class ItemAction : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string State { get; set; } = "State";
        public int Value { get; set; } = 0;

        // Navigation property
        public virtual Item Item { get; set; }
    }
}
