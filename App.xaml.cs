using System;
using System.Configuration;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SalesMan.Repositories;
using SalesMan.Services;
using SalesMan.ViewModels;

namespace SalesMan;

public partial class App : Application
{
    #region properties

    public IServiceProvider? ServiceProvider { get; set; }

    #endregion

    #region private methods

    private static IServiceProvider ConfigureServices()
    {
        IServiceCollection services = new ServiceCollection();

        // views
        services.AddSingleton<MainWindow>();

        // services
        services.AddSingleton<IDialogService, DefaultDialogService>();
        services.AddSingleton<IPrintService, DefaultPrintService>();

        // viewModels
        services.AddSingleton<IMainViewModel, DefaultMainViewModel>();
        services.AddSingleton<ICustomerDialogViewModel, DefaultCustomerDialogViewModel>();
        services.AddSingleton<ICustomerManagerViewModel, DefaultCustomerManagerViewModel>();
        services.AddSingleton<IItemDialogViewModel, DefaultItemDialogViewModel>();
        services.AddSingleton<IItemManagerViewModel, DefaultItemManagerViewModel>();
        services.AddSingleton<IOrderDialogViewModel, DefaultOrderDialogViewModel>();
        services.AddSingleton<IOrderManagerViewModel, DefaultOrderManagerViewModel>();

        // repositories
        switch (ConfigurationManager.AppSettings["dbType"]?.ToLower())
        {
            case "fs":
                services.AddSingleton<ICustomerRepository, FsCustomerRepository>();
                services.AddSingleton<IItemRepository, FsItemRepository>();
                services.AddSingleton<IOrderRepository, FsOrderRepository>();
                break;
            case "sql":
                services.AddSingleton<ICustomerRepository, SqlCustomerRepository>();
                services.AddSingleton<IItemRepository, SqlItemRepository>();
                services.AddSingleton<IOrderRepository, SqlOrderRepository>();
                break;
        }

        return services.BuildServiceProvider();
    }

    #endregion

    #region handlers

    protected override void OnStartup(StartupEventArgs e)
    {
        // ioc
        this.ServiceProvider = ConfigureServices();

        // show ui
        var mainWindow = this.ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (this.ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    #endregion
}
