using CommonServiceLocator;
using DragonAsia.SalesMan.Repositories;
using GalaSoft.MvvmLight.Ioc;

namespace DragonAsia.SalesMan.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			SimpleIoc.Default.Register<ICustomerRepository, CustomerRepository>();
			SimpleIoc.Default.Register<IItemRepository, ItemRepository>();
			SimpleIoc.Default.Register<IOrderRepository, OrderRepository>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ItemManagerViewModel>();
            SimpleIoc.Default.Register<CustomerManagerViewModel>();
            SimpleIoc.Default.Register<OrderManagerViewModel>();
        }

        public MainViewModel MainVM { get { return ServiceLocator.Current.GetInstance<MainViewModel>(); } }
        public ItemManagerViewModel ItemManagerVM { get { return ServiceLocator.Current.GetInstance<ItemManagerViewModel>(); } }
        public CustomerManagerViewModel CustomerManagerVM { get { return ServiceLocator.Current.GetInstance<CustomerManagerViewModel>(); } }
        public OrderManagerViewModel OrderManagerVM { get { return ServiceLocator.Current.GetInstance<OrderManagerViewModel>(); } }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}