using Microsoft.VisualBasic;

namespace SmartHomeProject.Models
{
    public class Item : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string MacIp { get; set; }
        public bool IsAssigned { get; set; }
        public int LastVersionNumber { get; set; }
        public bool DoUpdateNow { get; set; } = false;
        public Guid? UserId { get; set; }
        public Guid? RoomId { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<ItemAction> Actions { get; set; }
        public virtual ICollection<VoiceCommand> VoiceCommands { get; set; }
    }
}
