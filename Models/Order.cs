using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SalesMan.Models;

public partial class Order : ObservableObject
{
    public Order()
    {
        this.CartItems.CollectionChanged += CartItems_CollectionChanged;
    }

    public Order(Order? entity)
    {
        if (entity == null)
        {
            return;
        }

        this.Id = entity.Id;
        this.DeliveryFee = entity.DeliveryFee;
        this.Timestamp = entity.Timestamp;
        this.Customer = new Customer(entity.Customer);
        this.CartItems = [.. entity.CartItems.ToList().ConvertAll(ci => new CartItem(ci))];

        this.CartItems.CollectionChanged += CartItems_CollectionChanged;
    }

    public string Id { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private double deliveryFee;

    public DateTime Timestamp { get; set; }

    [ObservableProperty]
    private Customer customer = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private ObservableCollection<CartItem> cartItems = [];

    public double TotalPrice => DeliveryFee + CartItems.Sum(ci => ci.Price * ci.Quantity);

    [JsonIgnore]
    public DateTime Group => new(Timestamp.Year, Timestamp.Month, Timestamp.Day);

    // ensure change to individual cartItem notifies change to TotalPrice
    public void AddCartItem(CartItem ci)
    {
        CartItems.Add(ci);
        ci.PropertyChanged += CartItem_PropertyChanged;
    }

    public void DeleteCartItem(CartItem ci)
    {
        CartItems.Remove(ci);
        ci.PropertyChanged -= CartItem_PropertyChanged;
    }

    #region handlers

    // fires when item is added/deleted
    private void CartItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(TotalPrice));
    }

    // fires when item is edited
    private void CartItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CartItem.Quantity))
        {
            OnPropertyChanged(nameof(TotalPrice));
        }
    }

    #endregion
}
