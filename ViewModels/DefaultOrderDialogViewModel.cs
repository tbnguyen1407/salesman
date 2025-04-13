using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SalesMan.Events;
using SalesMan.Models;
using SalesMan.Repositories;
using SalesMan.Services;

namespace SalesMan.ViewModels;

public partial class DefaultOrderDialogViewModel : ObservableRecipient, IOrderDialogViewModel, IRecipient<EntityDialogOpenEvent<Order>>, IRecipient<EntityStorageUpdatedEvent<Customer>>, IRecipient<EntityStorageUpdatedEvent<Item>>
{
    #region fields

    private readonly ICustomerRepository rCustomer;
    private readonly IItemRepository rItem;
    private readonly IOrderRepository rOrder;
    private readonly IPrintService sPrint;

    #endregion

    public DefaultOrderDialogViewModel(ICustomerRepository rCustomer, IItemRepository rItem, IOrderRepository rOrder, IPrintService sPrint)
    {
        this.sPrint = sPrint;
        this.rCustomer = rCustomer;
        this.rItem = rItem;
        this.rOrder = rOrder;

        // register message handlers
        Messenger.RegisterAll(this);

        // init
        _ = Refresh();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCartItemCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCartItemCommand))]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private EntityAction action;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private Order entity = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private bool isCustomerNew;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredCustomers))]
    private IEnumerable<Customer> customers = [];

    public ObservableCollection<Customer> FilteredCustomers
    {
        get
        {
            string filterStr = CustomerFilter.ToLower().Trim();
            return [.. Customers.Where(c =>
                        c.Name.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase) ||
                        c.Phone.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase) ||
                        c.Address.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase))];
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private Customer? curCustomer;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private string? newCustomerName;

    [ObservableProperty]
    private string? newCustomerAddress;

    [ObservableProperty]
    private string? newCustomerEmail;

    [ObservableProperty]
    private string? newCustomerPhone;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredCustomers))]
    private string customerFilter = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredItems))]
    private IEnumerable<Item> items = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCartItemCommand))]
    private Item? item;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredItems))]
    private string itemFilter = string.Empty;

    public ObservableCollection<Item> FilteredItems
    {
        get
        {
            string filterStr = ItemFilter.ToLower().Trim();
            return [.. Items.Where(i =>
                i.Code.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase) ||
                i.Name.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase) ||
                i.Description.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase))];
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCartItemCommand))]
    private CartItem? cartItem;

    [ObservableProperty]
    private bool includeTaxRef = true;

    [ObservableProperty]
    private bool printReceipt = true;

    [RelayCommand(CanExecute = nameof(ConfirmCanExecute))]
    private async Task Confirm()
    {
        switch (Action)
        {
            case EntityAction.Create:
                // save new customer
                if (IsCustomerNew)
                {
                    var newCustomer = new Customer
                    {
                        Name = NewCustomerName!,
                        Address = NewCustomerAddress,
                        Email = NewCustomerEmail,
                        Phone = NewCustomerPhone,
                    };
                    await rCustomer.CreateAsync(newCustomer);
                    Messenger.Send(new EntityStorageUpdatedEvent<Customer>());

                    Entity.Customer = newCustomer;
                }
                else
                {
                    Entity.Customer = CurCustomer!;
                }

                // save order
                Entity.Timestamp = DateTime.Now;

                await rOrder.CreateAsync(Entity);
                Messenger.Send(new EntityStorageUpdatedEvent<Order>());

                // print receipt
                if (PrintReceipt)
                {
                    sPrint.Print(Entity);
                }
                break;
        }

        // notify
        Messenger.Send(new EntityDialogFinishedEvent<Order>());
    }

    private bool ConfirmCanExecute()
    {
        // check customer
        if (IsCustomerNew)
        {
            if (string.IsNullOrWhiteSpace(NewCustomerName))
            {
                return false;
            }
        }
        else
        {
            if (CurCustomer == null)
            {
                return false;
            }
        }

        // check cart
        if (Entity.CartItems.Count == 0)
        {
            return false;
        }

        return true;
    }

    [RelayCommand(CanExecute = nameof(AddCartItemCanExecute))]
    private void AddCartItem()
    {
        CartItem existingItemInCart = Entity.CartItems.FirstOrDefault(i => i.Code == Item.Code);
        if (existingItemInCart != null)
        {
            existingItemInCart.Quantity++;
        }
        else
        {
            CartItem newItem = new()
            {
                Id = Item.Id,
                Code = Item.Code,
                Name = Item.Name,
                Price = Item.Price,
                Quantity = 1,
            };
            Entity.AddCartItem(newItem);
        }
    }

    private bool AddCartItemCanExecute()
    {
        if (Action == EntityAction.View)
        {
            return false;
        }
        if (Item == null)
        {
            return false;
        }

        return true;
    }

    [RelayCommand(CanExecute = nameof(DeleteCartItemCanExecute))]
    private void DeleteCartItem()
    {
        CartItem? existingItemInCart = Entity.CartItems.FirstOrDefault(ci => ci.Code == CartItem!.Code);
        if (existingItemInCart != null)
        {
            if (existingItemInCart.Quantity > 1)
            {
                existingItemInCart.Quantity--;
            }
            else
            {
                Entity.DeleteCartItem(existingItemInCart);
            }
        }
    }

    private bool DeleteCartItemCanExecute()
    {
        if (Action == EntityAction.View)
        {
            return false;
        }
        if (CartItem == null)
        {
            return false;
        }

        return true;
    }

    [RelayCommand]
    private async Task Refresh()
    {
        Customers = await rCustomer.RetrieveAllAsync();
        Items = await rItem.RetrieveAllAsync();
    }

    #region handlers

    public void Receive(EntityDialogOpenEvent<Order> message)
    {
        Action = message.Action;
        switch (message.Action)
        {
            case EntityAction.Create:
                Entity = new Order();
                IsCustomerNew = true;
                NewCustomerName = null;
                NewCustomerAddress = null;
                NewCustomerEmail = null;
                NewCustomerPhone = null;
                break;
            case EntityAction.Edit:
                Entity = message.Entity!;
                IsCustomerNew = false;
                CurCustomer = Customers.FirstOrDefault(c => c.Id.Equals(message.Entity!.Customer.Id));
                break;
            case EntityAction.View:
                Entity = message.Entity!;

                // set customer as new so the info is expanded
                IsCustomerNew = true;
                NewCustomerName = message.Entity?.Customer.Name;
                NewCustomerAddress = message.Entity?.Customer.Address;
                NewCustomerEmail = message.Entity?.Customer.Email;
                NewCustomerPhone = message.Entity?.Customer.Phone;
                break;
        }
    }

    public void Receive(EntityStorageUpdatedEvent<Customer> message)
    {
        _ = Refresh();
    }

    public void Receive(EntityStorageUpdatedEvent<Item> message)
    {
        _ = Refresh();
    }

    #endregion
}
