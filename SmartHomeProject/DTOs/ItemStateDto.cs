namespace SmartHomeProject.DTOs
{
    public class ItemStateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ItemActionDto> States { get; set; } = new List<ItemActionDto>();
    }

    public class ItemActionDto
    {
        public string State { get; set; }
        public int Value { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
