using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SalesMan.Events;
using SalesMan.Models;
using SalesMan.Repositories;

namespace SalesMan.ViewModels;

partial class DefaultCustomerDialogViewModel : ObservableRecipient, ICustomerDialogViewModel, IRecipient<EntityDialogOpenEvent<Customer>>
{
    #region fields

    private readonly ICustomerRepository rCustomer;

    #endregion

    public DefaultCustomerDialogViewModel(ICustomerRepository rCustomer)
    {
        this.rCustomer = rCustomer;

        // register message handlers
        Messenger.RegisterAll(this);
    }

    #region properties

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private EntityAction action;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private Customer entity = new();

    #endregion

    #region commands

    [RelayCommand(CanExecute = nameof(ConfirmCanExecute))]
    private async Task Confirm()
    {
        switch (Action)
        {
            case EntityAction.Create:
                await rCustomer.CreateAsync(Entity);
                Messenger.Send(new EntityStorageUpdatedEvent<Customer>());
                break;
            case EntityAction.Edit:
                await rCustomer.UpdateAsync(Entity.Id, Entity);
                Messenger.Send(new EntityStorageUpdatedEvent<Customer>());
                break;
        }

        // notify
        Messenger.Send(new EntityDialogFinishedEvent<Customer>());
    }

    private bool ConfirmCanExecute()
    {
        switch (Action)
        {
            case EntityAction.Create:
            case EntityAction.Edit:
                if (string.IsNullOrWhiteSpace(Entity.Name))
                {
                    return false;
                }

                break;
        }

        return true;
    }

    #endregion

    # region handlers

    public void Receive(EntityDialogOpenEvent<Customer> message)
    {
        Action = message.Action;
        Entity = Action == EntityAction.Create ? new Customer() : message.Entity!;
    }

    #endregion
}
