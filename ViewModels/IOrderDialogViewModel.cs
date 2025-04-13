using CommunityToolkit.Mvvm.Input;

namespace SalesMan.ViewModels;

public interface IOrderDialogViewModel
{
    public IAsyncRelayCommand ConfirmCommand { get; }
    public IRelayCommand AddCartItemCommand { get; }
    public IRelayCommand DeleteCartItemCommand { get; }
}
