using DragonAsia.SalesMan.Event;
using DragonAsia.SalesMan.Models;
using DragonAsia.SalesMan.Repositories;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DragonAsia.SalesMan.ViewModel
{
    class ItemDialogViewModel : ViewModelBase
    {
		private IItemRepository repository;

        public ItemDialogViewModel(IItemRepository repo, Item template)
        {
			repository = repo;

            // initialize commands
            CmdConfirm = new RelayCommand(CmdConfirmExecute, CmdConfirmCanExecute);

            // initialize categories
            Categories = File.ReadAllLines("item_categories.txt").ToList();

            IsNew = template == null;
            Entity = IsCur ? template : new Item();
        }

        #region properties

        public bool IsNew { get; set; }
        public bool IsCur { get { return !IsNew; } }

        private Item entity;
        public Item Entity
        {
            get { return entity; }
            set { Set("Entity", ref entity, value); }
        }
            
        private List<string> categories;    
        public List<string> Categories
        {
            get { return categories; }
            set { Set("Categories", ref categories, value); }
        }

        #endregion

        #region commands

        public RelayCommand CmdConfirm { get; private set; }

        #endregion

        #region command implementations

        private bool CmdConfirmCanExecute()
        {
            if (string.IsNullOrWhiteSpace(Entity.Code))
                return false;
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

			Messenger.Default.Send(new ItemStorageUpdatedEvent());
            Messenger.Default.Send(new ItemDialogFinishedEvent());
        }

        #endregion
    }
}