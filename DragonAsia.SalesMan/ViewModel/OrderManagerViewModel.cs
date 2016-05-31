using DragonAsia.SalesMan.Event;
using DragonAsia.SalesMan.Helper;
using DragonAsia.SalesMan.Models;
using DragonAsia.SalesMan.Properties;
using DragonAsia.SalesMan.Repositories;
using DragonAsia.SalesMan.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DragonAsia.SalesMan.ViewModel
{
    public class OrderManagerViewModel : ViewModelBase
    {
		private ICustomerRepository cusRepository;
		private IItemRepository itmRepository;
		private IOrderRepository repository;
		
        public OrderManagerViewModel(ICustomerRepository cusRepo, IItemRepository itmRepo, IOrderRepository repo)
        {
            if (!IsInDesignMode)
            {
				repository = repo;
				cusRepository = cusRepo;
				itmRepository = itmRepo;
				
                // initialize commands
                CmdAdd = new RelayCommand(CmdAddExecute);
                CmdDelete = new RelayCommand(CmdDeleteExecute, () => false);
                CmdEdit = new RelayCommand(CmdEditExecute, () => false);
                CmdPrint = new RelayCommand(CmdPrintExecute, () => Entity != null);

                // register events
                Messenger.Default.Register<OrderStorageUpdatedEvent>(this, evt => CmdRefreshExecute());
                
                // initial load
                CmdRefreshExecute();
            }
        }

		#region properties

		private DateTime fr = DateTime.Now.AddMonths(-1);
		public DateTime Fr
		{
			get { return fr; }
			set
			{
				if (Set("Fr", ref fr, value))
					CmdRefreshExecute();
			}
		}

		private DateTime to = DateTime.Now.AddMonths(1);
		public DateTime To
		{
			get { return to; }
			set
			{
				if (Set("To", ref to, value))
					CmdRefreshExecute();
			}
		}

        private List<Order> entities = new List<Order>();
        public List<Order> Entities
        {
            get { return entities; }
            set
            {
                Set("Entities", ref entities, value);
                RaisePropertyChanged("FilteredEntities");
            }
        }

        public IEnumerable<Order> FilteredEntities
        {
            get
            {
                string filterStr = filter.ToLower().Trim();
                return entities.Where(u => u.Guid.Contains(filterStr)
                    || u.Customer.Name.Contains(filterStr));
            }
        }

        private Order entity;
        public Order Entity
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
        public RelayCommand CmdPrint { get; private set; }

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

        private void CmdPrintExecute()
        {
            if (Entity == null)
                return;

            MessageBoxResult confirm = MessageBox.Show("Print selected entry?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.OK)
                return;

            PrinterHelper.Print(Entity);
        }

        private void CmdRefreshExecute()
        {
            Task.Run(() =>
            {
                if (Settings.Default.taxref_exclude_empty)
                    Entities = repository.RetrieveAll(Fr.Date, To.Date.AddDays(1).AddSeconds(-1)).Where(e => e.TaxRefIncluded).OrderByDescending(e => e.Timestamp).ToList();
                else
                    Entities = repository.RetrieveAll(Fr.Date, To.Date.AddDays(1).AddSeconds(-1)).OrderByDescending(e => e.Timestamp).ToList();
            });
        }

        #endregion

        #region private methods

        private void ShowDialog(Order template)
        {
            var wnd = new Window
            {
                Owner = Application.Current.MainWindow,
                Width = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.Height,
                ResizeMode = ResizeMode.NoResize,
                Content = new OrderDialogView(itmRepository) { DataContext = new OrderDialogViewModel(cusRepository, itmRepository, repository, template) }
            };
            wnd.Loaded += (sender, args) => Messenger.Default.Register<OrderDialogFinishedEvent>(wnd, evt => wnd.Close());
            wnd.ShowDialog();
        }

        #endregion
    }
}
