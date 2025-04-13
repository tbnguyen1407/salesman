using System;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using SalesMan.Events;
using SalesMan.Models;
using SalesMan.ViewModels;
using SalesMan.Views;

namespace SalesMan.Services;

public class DefaultDialogService : IDialogService
{
    public bool ShowConfirmation(string title, string message)
    {
        MessageBoxResult mbResult = MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        return mbResult == MessageBoxResult.OK;
    }

    public void ShowMessage(string title, string message)
    {
        MessageBox.Show(message, title);
    }

    public void ShowEntityDialog<T>(EntityAction action, T value)
    {
        UserControl userControl = new UserControl();
        string title = string.Empty;
        IServiceProvider serviceProvider = ((App)Application.Current).ServiceProvider!;

        switch (value)
        {
            case Customer:
                userControl = new CustomerDialogView { DataContext = serviceProvider.GetService(typeof(ICustomerDialogViewModel)) };
                break;
            case Item:
                userControl = new ItemDialogView { DataContext = serviceProvider.GetService(typeof(IItemDialogViewModel)) };
                break;
            case Order:
                userControl = new OrderDialogView { DataContext = serviceProvider.GetService(typeof(IOrderDialogViewModel)) };
                break;
        }

        switch (action)
        {
            case EntityAction.Create:
                title = "New";
                break;
            case EntityAction.Edit:
                title = "Edit";
                break;
            case EntityAction.View:
                title = "Info";
                break;
        }

        WeakReferenceMessenger.Default.Send(new EntityDialogOpenEvent<T>(action, value));

        var wnd = new Window
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            SizeToContent = SizeToContent.WidthAndHeight,
            Content = userControl,
            Title = title,
        };
        wnd.Loaded += (_, _) => WeakReferenceMessenger.Default.Register<EntityDialogFinishedEvent<T>>(wnd, (_, _) => wnd.Close());
        wnd.ShowDialog();
    }
}
