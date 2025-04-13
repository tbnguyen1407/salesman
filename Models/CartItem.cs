using CommunityToolkit.Mvvm.ComponentModel;

namespace SalesMan.Models;

public partial class CartItem : ObservableObject
{
    public CartItem()
    {
    }

    public CartItem(CartItem? entity)
    {
        if (entity == null)
        {
            return;
        }

        this.Id = entity.Id;
        this.Code = entity.Code;
        this.Name = entity.Name;
        this.Price = entity.Price;
        this.Quantity = entity.Quantity;
    }

    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }

    [ObservableProperty]
    private int quantity;
}
