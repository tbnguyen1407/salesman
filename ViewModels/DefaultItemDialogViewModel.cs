using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SalesMan.Events;
using SalesMan.Models;
using SalesMan.Repositories;

namespace SalesMan.ViewModels;

partial class DefaultItemDialogViewModel : ObservableRecipient, IItemDialogViewModel, IRecipient<EntityDialogOpenEvent<Item>>
{
    #region fields

    private readonly IItemRepository rItem;

    #endregion

    public DefaultItemDialogViewModel(IItemRepository rItem)
    {
        this.rItem = rItem;

        // register message handlers
        Messenger.RegisterAll(this);

        // initialize
        Categories = new ReadOnlyCollection<string>(File.ReadAllLines("extras/item_categories.txt"));
    }

    #region properties

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private EntityAction action;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private Item entity = new();

    [ObservableProperty]
    private IReadOnlyCollection<string> categories;

    #endregion

    #region commands

    [RelayCommand(CanExecute = nameof(ConfirmCanExecute))]
    private async Task Confirm()
    {
        switch (Action)
        {
            case EntityAction.Create:
                await rItem.CreateAsync(Entity);
                Messenger.Send(new EntityStorageUpdatedEvent<Item>());
                break;
            case EntityAction.Edit:
                await rItem.UpdateAsync(Entity.Id, Entity);
                Messenger.Send(new EntityStorageUpdatedEvent<Item>());
                break;
        }

        // notify
        Messenger.Send(new EntityDialogFinishedEvent<Item>());
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

                if (string.IsNullOrWhiteSpace(Entity.Code))
                {
                    return false;
                }

                break;
        }

        return true;
    }

    #endregion

    #region handlers

    public void Receive(EntityDialogOpenEvent<Item> message)
    {
        Action = message.Action;
        Entity = Action == EntityAction.Create ? new Item() : message.Entity!;
    }

    #endregion
}
