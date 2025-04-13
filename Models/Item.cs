using System.Text.Json.Serialization;

namespace SalesMan.Models;

public class Item
{
    public Item()
    {
    }

    public Item(Item? entity)
    {
        if (entity == null)
        {
            return;
        }

        this.Id = entity.Id;
        this.Code = entity.Code;
        this.Name = entity.Name;
        this.Description = entity.Description;
        this.Category = entity.Category;
        this.Price = entity.Price;
    }

    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public double Price { get; set; }

    [JsonIgnore]
    public string Group => Category;
}
