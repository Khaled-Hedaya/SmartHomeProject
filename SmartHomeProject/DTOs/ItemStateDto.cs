namespace SmartHomeProject.DTOs
{
    public class ItemStateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<StateInfo> States { get; set; }
    }

    public class StateInfo
    {
        public string State { get; set; }
        public string Value { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
