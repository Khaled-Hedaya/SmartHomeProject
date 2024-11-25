namespace SmartHomeProject.Models
{
    public class Product : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal ElectricityConsumption { get; set; }
        public string ImageUrl { get; set; }
        public int LastVersionNumber { get; set; }
        public string LastVersionUrl { get; set; }

        // Navigation properties
        public virtual ICollection<ProductAction> Actions { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}
