using SmartHomeProject.Models;

public class User : BaseEntity
{
    public User()
    {
        // Initialize collections in constructor
        Items = new List<Item>();
        Complaints = new List<Complaint>();
    }

    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Image { get; set; }

    // Navigation properties
    public virtual ICollection<Item> Items { get; set; }
    public virtual ICollection<Complaint> Complaints { get; set; }
}