using System.Windows;

namespace DragonAsia.SalesMan.View
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();

            hpOrders_Click(null, null);
        }

        private void hpItems_Click(object sender, RoutedEventArgs e)
        {
            sesItems.Visibility = Visibility.Visible;
            sesCustomers.Visibility = Visibility.Collapsed;
            sesOrders.Visibility = Visibility.Collapsed;
        }

        private void hpCustomers_Click(object sender, RoutedEventArgs e)
        {
            sesItems.Visibility = Visibility.Collapsed;
            sesCustomers.Visibility = Visibility.Visible;
            sesOrders.Visibility = Visibility.Collapsed;
        }

        private void hpOrders_Click(object sender, RoutedEventArgs e)
        {
            sesItems.Visibility = Visibility.Collapsed;
            sesCustomers.Visibility = Visibility.Collapsed;
            sesOrders.Visibility = Visibility.Visible;
        }
    }
}
