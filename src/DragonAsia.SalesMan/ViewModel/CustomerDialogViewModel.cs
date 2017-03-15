using DragonAsia.SalesMan.Event;
using DragonAsia.SalesMan.Models;
using DragonAsia.SalesMan.Repositories;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace DragonAsia.SalesMan.ViewModel
{
    class CustomerDialogViewModel : ViewModelBase
    {
		private ICustomerRepository repository;

        public CustomerDialogViewModel(ICustomerRepository repo, Customer template)
        {
			repository = repo;

            // initialize commands
            CmdConfirm = new RelayCommand(CmdConfirmExecute, CmdConfirmCanExecute);

            IsNew = template == null;
            Entity = IsCur ? template : new Customer();
        }

        #region properties

        public bool IsNew { get;set; }
        public bool IsCur { get { return !IsNew; } }

        private Customer entity;
        public Customer Entity
        {
            get { return entity; }
            set { Set("Entity", ref entity, value); }
        }

        #endregion

        #region commands

        public RelayCommand CmdConfirm { get; private set; }

        #endregion

        #region command implementations

        private bool CmdConfirmCanExecute()
        {
            if (string.IsNullOrWhiteSpace(Entity.Name))
                return false;
            return true;
        }

        private void CmdConfirmExecute()
        {
			if (IsNew)
				repository.Create(Entity);
			else
				repository.Update(Entity);

			Messenger.Default.Send(new CustomerStorageUpdatedEvent());
            Messenger.Default.Send(new CustomerDialogFinishedEvent());
        }

        #endregion
    }
}
