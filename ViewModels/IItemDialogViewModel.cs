using CommunityToolkit.Mvvm.Input;

namespace SalesMan.ViewModels;

public interface IItemDialogViewModel
{
    public IAsyncRelayCommand ConfirmCommand { get; }
}
