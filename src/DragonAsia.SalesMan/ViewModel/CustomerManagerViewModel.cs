using DragonAsia.SalesMan.Event;
using DragonAsia.SalesMan.Models;
using DragonAsia.SalesMan.Repositories;
using DragonAsia.SalesMan.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DragonAsia.SalesMan.ViewModel
{
    public class CustomerManagerViewModel : ViewModelBase
    {
		private ICustomerRepository repository;

        public CustomerManagerViewModel(ICustomerRepository repo)
        {
            if (!IsInDesignMode)
            {
				repository = repo;

                // initialize commands
                CmdAdd = new RelayCommand(CmdAddExecute);
                CmdDelete = new RelayCommand(CmdDeleteExecute, () => Entity != null);
                CmdEdit = new RelayCommand(CmdEditExecute, () => Entity != null);

                // register events
                Messenger.Default.Register<CustomerStorageUpdatedEvent>(this, evt => CmdRefreshExecute());
                
                // initial load
                CmdRefreshExecute();
            }
        }

        #region properties

        private List<Customer> entities = new List<Customer>();
        public List<Customer> Entities
        {
            get { return entities; }
            set
            {
                Set("Entities", ref entities, value);
                RaisePropertyChanged("FilteredEntities");
            }
        }

        public IEnumerable<Customer> FilteredEntities
        {
            get
            {
                string filterStr = filter.ToLower().Trim();
                return entities.Where(e => e.Guid.Contains(filterStr)
                    || e.Name.ToLower().Contains(filterStr)
                    || e.Phone.Contains(filterStr)
                    || e.Address.ToLower().Contains(filterStr));
            }
        }

        private Customer entity;
        public Customer Entity
        {
            get { return entity; }
            set { Set("Entity", ref entity, value); }
        }

        private string filter = string.Empty;
        public string Filter
        {
            get { return filter; }
            set
            {
                Set("Filter", ref filter, value);
                RaisePropertyChanged("FilteredEntities");
            }
        }

        #endregion

        #region commands

        public RelayCommand CmdAdd { get; private set; }
        public RelayCommand CmdDelete { get; private set; }
        public RelayCommand CmdEdit { get; private set; }

        #endregion

        #region command implementations

        private void CmdAddExecute()
        {
            ShowDialog(null);
        }

        private void CmdEditExecute()
        {
            ShowDialog(Entity);
        }

        private void CmdDeleteExecute()
        {
            if (Entity == null)
                return;

            MessageBoxResult confirm = MessageBox.Show("Delete selected entry?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.OK)
                return;

			repository.Delete(Entity.Guid);
            CmdRefreshExecute();
        }

        private void CmdRefreshExecute()
        {
            Task.Run(() => Entities = repository.RetrieveAll().OrderBy(e => e.Name).ToList());
        }

        #endregion

        #region private methods

        private void ShowDialog(Customer template)
        {
            var wnd = new Window
            {
                Owner = Application.Current.MainWindow,
                Width = 500,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.Height,
                ResizeMode = ResizeMode.NoResize,
                Content = new CustomerDialogView { DataContext = new CustomerDialogViewModel(repository, template) }
            };
            wnd.Loaded += (sender, args) => Messenger.Default.Register<CustomerDialogFinishedEvent>(wnd, evt => wnd.Close());
            wnd.ShowDialog();
        }

        #endregion

    }
}
