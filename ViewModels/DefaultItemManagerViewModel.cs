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

public partial class DefaultItemManagerViewModel : ObservableRecipient, IItemManagerViewModel, IRecipient<EntityStorageUpdatedEvent<Item>>
{
    #region fields

    private readonly IItemRepository rItem;
    private readonly IDialogService sDialog;

    #endregion

    public DefaultItemManagerViewModel(IItemRepository rItem, IDialogService sDialog)
    {
        this.rItem = rItem;
        this.sDialog = sDialog;

        // register message handlers
        Messenger.RegisterAll(this);

        // initialize data
        Messenger.Send(new EntityStorageUpdatedEvent<Item>());
    }

    #region properties

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredEntities))]
    private IEnumerable<Item> entities = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    [NotifyCanExecuteChangedFor(nameof(ViewCommand))]
    private Item? entity;

    public ObservableCollection<Item> FilteredEntities
    {
        get
        {
            string filterStr = Filter.Trim();
            return string.IsNullOrEmpty(filterStr)
                ? [.. Entities]
                : [.. Entities.Where(i =>
                    i.Code.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase) ||
                    i.Name.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase) ||
                    i.Description.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase))];
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
        sDialog.ShowEntityDialog(EntityAction.Create, new Item());
    }

    [RelayCommand(CanExecute = nameof(ViewCanExecute))]
    private void View()
    {
        sDialog.ShowEntityDialog(EntityAction.View, new Item(Entity));
    }

    private bool ViewCanExecute()
    {
        return Entity != null;
    }

    [RelayCommand(CanExecute = nameof(EditCanExecute))]
    private void Edit()
    {
        sDialog.ShowEntityDialog(EntityAction.Edit, new Item(Entity));
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
            await rItem.DeleteAsync(Entity.Id);

            // notify
            Messenger.Send(new EntityStorageUpdatedEvent<Item>());
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
        var results = await rItem.RetrieveAllAsync();
        Entities = results.OrderBy(i => i.Code);
    }

    #endregion

    #region handlers

    public void Receive(EntityStorageUpdatedEvent<Item> message)
    {
        _ = Refresh();
    }

    #endregion
}
