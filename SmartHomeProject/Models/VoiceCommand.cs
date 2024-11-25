namespace SmartHomeProject.Models
{
    public class VoiceCommand : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ActionName { get; set; }
        public string ActionState { get; set; }
        public string Command { get; set; }

        // Navigation property
        public virtual Item Item { get; set; }
    }
}
