using DragonAsia.SalesMan.Event;
using DragonAsia.SalesMan.Repositories;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DragonAsia.SalesMan.View
{
	public partial class OrderDialogView
    {
		// todo: inject this
		private IItemRepository itmRepository;

        public OrderDialogView(IItemRepository itmRepo)
        {
			itmRepository = itmRepo;
            InitializeComponent();
        }

        private void tbxMenuItemFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string selectedCode = tbxMenuItemFilter.Text.ToLower();
                var existing = itmRepository.RetrieveAll().FirstOrDefault(it => it.Code.ToLower() == selectedCode);

                if (existing != null)
                    Messenger.Default.Send(new ItemSelectedEvent { Entity = existing });
                else
                    MessageBox.Show("Item code not found");

                tbxMenuItemFilter.Clear();
            }
        }
    }
}
