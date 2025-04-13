using CommunityToolkit.Mvvm.Input;
using SalesMan.Models;

namespace SalesMan.ViewModels;

public interface ICustomerDialogViewModel
{
    public EntityAction Action { get; }
    public Customer Entity { get; }

    public IAsyncRelayCommand ConfirmCommand { get; }
}
