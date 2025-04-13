using System.Text.Json.Serialization;

namespace SalesMan.Models;

public class Customer
{
    public Customer()
    {
    }

    public Customer(Customer? entity)
    {
        if (entity == null)
        {
            return;
        }

        this.Id = entity.Id;
        this.Name = entity.Name;
        this.Address = entity.Address;
        this.Email = entity.Email;
        this.Phone = entity.Phone;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    [JsonIgnore]
    public string Group => (Name??"-")[..1].ToUpperInvariant();
}
