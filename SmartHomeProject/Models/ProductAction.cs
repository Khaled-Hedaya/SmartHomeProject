namespace SmartHomeProject.Models
{
    public class ProductAction : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string State { get; set; } = "State";
        public int Value { get; set; } = 0;

        // Navigation property
        public virtual Product Product { get; set; }
    }
}
