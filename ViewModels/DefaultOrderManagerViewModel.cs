using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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

partial class DefaultOrderManagerViewModel : ObservableRecipient, IOrderManagerViewModel, IRecipient<EntityStorageUpdatedEvent<Order>>
{
    #region fields

    private readonly IOrderRepository rOrder;
    private readonly IDialogService sDialog;
    private readonly IPrintService sPrint;

    #endregion

    public DefaultOrderManagerViewModel(IOrderRepository rOrder, IDialogService sDialog, IPrintService sPrint)
    {
        this.rOrder = rOrder;
        this.sDialog = sDialog;
        this.sPrint = sPrint;

        // register message handlers
        Messenger.RegisterAll(this);

        // initialize data
        Messenger.Send(new EntityStorageUpdatedEvent<Order>());
    }

    #region properties

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredEntities))]
    private IEnumerable<Order> entities = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    [NotifyCanExecuteChangedFor(nameof(PrintCommand))]
    [NotifyCanExecuteChangedFor(nameof(ViewCommand))]
    private Order? entity;

    public ObservableCollection<Order> FilteredEntities
    {
        get
        {
            string filterStr = Filter.Trim();
            return string.IsNullOrEmpty(filterStr)
                ? [.. Entities]
                : [.. Entities.Where(o =>
                    o.Id.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase) ||
                    o.Customer.Name.Contains(filterStr, StringComparison.CurrentCultureIgnoreCase))];
        }
    }

    [ObservableProperty]
    private string filter = string.Empty;

    [ObservableProperty]
    private DateTime timeFilterFr = DateTime.Now.AddMonths(-1);

    [ObservableProperty]
    private DateTime timeFilterTo = DateTime.Now.AddMonths(1);

    #endregion

    #region commands

    [RelayCommand]
    private void Add()
    {
        sDialog.ShowEntityDialog(EntityAction.Create, new Order());
    }

    [RelayCommand(CanExecute = nameof(ViewCanExecute))]
    private void View()
    {
        sDialog.ShowEntityDialog(EntityAction.View, new Order(Entity));
    }

    private bool ViewCanExecute()
    {
        return Entity != null;
    }

    [RelayCommand(CanExecute = nameof(EditCanExecute))]
    private void Edit()
    {
        sDialog.ShowEntityDialog(EntityAction.Edit, new Order(Entity));
    }

    private bool EditCanExecute()
    {
        return false;
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
            await rOrder.DeleteAsync(Entity.Id);

            // notify
            Messenger.Send(new EntityStorageUpdatedEvent<Order>());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private bool DeleteCanExecute()
    {
        return bool.Parse((ConfigurationManager.AppSettings["uiOrderManagerAllowDelete"] ?? "false"));
    }

    [RelayCommand(CanExecute = nameof(PrintCanExecute))]
    private void Print()
    {
        if (Entity == null)
        {
            return;
        }

        // confirm
        bool confirmed = sDialog.ShowConfirmation("Confirmation", "Print receipt for selected entry?");
        if (!confirmed)
        {
            return;
        }

        // execute
        sPrint.Print(Entity);
    }

    private bool PrintCanExecute()
    {
        return Entity != null;
    }

    [RelayCommand]
    private async Task Refresh()
    {
        var results = await rOrder.RetrieveAllAsync(TimeFilterFr, TimeFilterTo);
        Entities = results.OrderByDescending(o => o.Timestamp);
    }

    #endregion

    #region handlers

    public void Receive(EntityStorageUpdatedEvent<Order> message)
    {
        _ = Refresh();
    }

    partial void OnTimeFilterFrChanged(DateTime value)
    {
        _ = Refresh();
    }

    partial void OnTimeFilterToChanged(DateTime value)
    {
        _ =  Refresh();
    }

    #endregion
}
