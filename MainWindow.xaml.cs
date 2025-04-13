using SalesMan.ViewModels;
using System.Windows;

namespace SalesMan;

public partial class MainWindow : Window
{
    public MainWindow(IMainViewModel mainViewModel)
    {
        InitializeComponent();

        this.MainView.DataContext = mainViewModel;
    }
}
