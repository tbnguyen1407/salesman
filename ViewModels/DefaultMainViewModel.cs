using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SalesMan.ViewModels;

public partial class DefaultMainViewModel(ICustomerManagerViewModel cm, IItemManagerViewModel im, IOrderManagerViewModel om) : ObservableObject, IMainViewModel
{
    #region properties

    [ObservableProperty]
    private ICustomerManagerViewModel vmCustomerManager = cm;

    [ObservableProperty]
    private IItemManagerViewModel vmItemManager = im;

    [ObservableProperty]
    private IOrderManagerViewModel vmOrderManager = om;

    [ObservableProperty]
    private bool customerManagerIsActive;

    [ObservableProperty]
    private bool itemManagerIsActive;

    [ObservableProperty]
    private bool orderManagerIsActive = true;

    #endregion

    #region command

    [RelayCommand]
    private void ActivateCustomerManager()
    {
        CustomerManagerIsActive = true;
        ItemManagerIsActive = false;
        OrderManagerIsActive = false;
    }

    [RelayCommand]
    private void ActivateItemManager()
    {
        CustomerManagerIsActive = false;
        ItemManagerIsActive = true;
        OrderManagerIsActive = false;
    }

    [RelayCommand]
    private void ActivateOrderManager()
    {
        CustomerManagerIsActive = false;
        ItemManagerIsActive = false;
        OrderManagerIsActive = true;
    }

    #endregion
}
