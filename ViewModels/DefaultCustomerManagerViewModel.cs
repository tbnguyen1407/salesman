using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SalesMan.Events;
using SalesMan.Models;
using SalesMan.Repositories;
using SalesMan.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SalesMan.ViewModels;

public partial class DefaultCustomerManagerViewModel : ObservableRecipient, ICustomerManagerViewModel, IRecipient<EntityStorageUpdatedEvent<Customer>>
{
    #region fields

    private readonly ICustomerRepository rCustomer;
    private readonly IDialogService sDialog;

    #endregion

    public DefaultCustomerManagerViewModel(ICustomerRepository rCustomer, IDialogService sDialog)
    {
        this.rCustomer = rCustomer;
        this.sDialog = sDialog;

        // register message handlers
        Messenger.RegisterAll(this);

        // initialize data
        Messenger.Send(new EntityStorageUpdatedEvent<Customer>());
    }

    #region properties

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredEntities))]
    private IEnumerable<Customer> entities = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    [NotifyCanExecuteChangedFor(nameof(ViewCommand))]
    private Customer? entity;

    public ObservableCollection<Customer> FilteredEntities
    {
        get
        {
            string filterStr = Filter.Trim();
            return string.IsNullOrEmpty(filterStr)
                ? [.. Entities]
                : [.. Entities.Where(c =>
                    c.Name.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase) ||
                    c.Phone.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase) ||
                    c.Address.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase))];
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredEntities))]
    private string filter = string.Empty;

    #endregion

    #region commands

    [RelayCommand]
    private void Add()
    {
        sDialog.ShowEntityDialog(EntityAction.Create, new Customer());
    }

    [RelayCommand(CanExecute = nameof(ViewCanExecute))]
    private void View()
    {
        sDialog.ShowEntityDialog(EntityAction.View, new Customer(Entity));
    }

    private bool ViewCanExecute()
    {
        return Entity != null;
    }

    [RelayCommand(CanExecute = nameof(EditCanExecute))]
    private void Edit()
    {
        sDialog.ShowEntityDialog(EntityAction.Edit, new Customer(Entity));
    }

    private bool EditCanExecute()
    {
        return Entity != null;
    }

    [RelayCommand(CanExecute = nameof(DeleteCanExecute))]
    private async Task Delete()
    {
        if (Entity == null)
        {
            return;
        }

        // confirm
        bool confirmed = sDialog.ShowConfirmation("Confirmation", "Delete selected entry?");
        if (!confirmed)
        {
            return;
        }

        try
        {
            // execute
            await rCustomer.DeleteAsync(Entity.Id);

            // notify
            Messenger.Send(new EntityStorageUpdatedEvent<Customer>());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private bool DeleteCanExecute()
    {
        return Entity != null;
    }

    [RelayCommand]
    private async Task Refresh()
    {
        var results = await rCustomer.RetrieveAllAsync();
        Entities = results.OrderBy(c => c.Name);
    }

    #endregion

    #region handlers

    public void Receive(EntityStorageUpdatedEvent<Customer> message)
    {
        _ = Refresh();
    }

    #endregion
}
