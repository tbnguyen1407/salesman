using CommunityToolkit.Mvvm.Input;

namespace SalesMan.ViewModels;

public interface IOrderManagerViewModel
{
    IRelayCommand AddCommand { get; }
    IRelayCommand ViewCommand { get; }
    IRelayCommand EditCommand { get; }
    IAsyncRelayCommand DeleteCommand { get; }
    IRelayCommand PrintCommand { get; }
}
