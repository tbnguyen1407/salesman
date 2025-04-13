using CommunityToolkit.Mvvm.Input;

namespace SalesMan.ViewModels;

public interface IMainViewModel
{
    public ICustomerManagerViewModel VmCustomerManager { get; }
    public IItemManagerViewModel VmItemManager { get; }
    public IOrderManagerViewModel VmOrderManager { get; }

    public IRelayCommand ActivateCustomerManagerCommand { get; }
    public IRelayCommand ActivateItemManagerCommand { get; }
    public IRelayCommand ActivateOrderManagerCommand { get; }
}
