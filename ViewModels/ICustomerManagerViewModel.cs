using CommunityToolkit.Mvvm.Input;

namespace SalesMan.ViewModels;

public interface ICustomerManagerViewModel
{
    IRelayCommand AddCommand { get; }
    IRelayCommand ViewCommand { get; }
    IRelayCommand EditCommand { get; }
    IAsyncRelayCommand DeleteCommand { get; }
}
