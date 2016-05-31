using DragonAsia.SalesMan.Event;
using DragonAsia.SalesMan.Helper;
using DragonAsia.SalesMan.Models;
using DragonAsia.SalesMan.Repositories;
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
    class OrderDialogViewModel : ViewModelBase
    {
		private ICustomerRepository cusRepository;
		private IItemRepository itmRepository;
		private IOrderRepository ordRepository;
		

        public OrderDialogViewModel(ICustomerRepository cusRepo, IItemRepository itmRepo, IOrderRepository ordRepo, Order template)
        {
			cusRepository = cusRepo;
			itmRepository = itmRepo;
			ordRepository = ordRepo;

            // initialize commands
            CmdAddMenuItem = new RelayCommand(CmdAddMenuItemExecute);
            CmdDeleteCartItem = new RelayCommand(CmdDeleteCartItemExecute);
            CmdConfirm = new RelayCommand(CmdConfirmExecute, CmdConfirmCanExecute);

            // register search
            Messenger.Default.Register<ItemSelectedEvent>(this, OnItemSelectedEvent);

            // intial refresh
            CmdRefreshExecute();

            IsNew = template == null;
            IsCustomerCur = true;

            Entity = IsCur ? template : new Order { Items = new List<Item>(), TaxRefIncluded = true, Timestamp = DateTime.Now };
        }

        #region properties

        public bool IsNew { get; set; }
        public bool IsCur { get { return !IsNew; } }

        private Order entity;
        public Order Entity
        {
            get { return entity; }
            set { Set("Entity", ref entity, value); }
        }

        #endregion

        #region properties - customer

        private bool isCustomerNew;
        public bool IsCustomerNew
        {
            get { return isCustomerNew; }
            set
            {
                if (Set("IsCustomerNew", ref isCustomerNew, value))
                {
                    isCustomerCur = !value;
                    RaisePropertyChanged("IsCustomerCur");
                }
            }
        }

        private bool isCustomerCur;
        public bool IsCustomerCur
        {
            get { return isCustomerCur; }
            set
            {
                if (Set("IsCustomerCur", ref isCustomerCur, value))
                {
                    isCustomerNew = !value;
                    RaisePropertyChanged("IsCustomerNew");
                }
            }
        }
        
        private List<Customer> customers = new List<Customer>();
        public List<Customer> Customers
        {
            get { return customers; }
            set
            {
                if (Set("Customers", ref customers, value))
                    RaisePropertyChanged("FilteredCustomers");
            }
        }

        private Customer curCustomer;
        public Customer CurCustomer
        {
            get { return curCustomer; }
            set { Set("CurCustomer", ref curCustomer, value); }
        }

        private Customer newCustomer = new Customer();
        public Customer NewCustomer
        {
            get { return newCustomer; }
            set { Set("NewCustomer", ref newCustomer, value); }
        }

        public IEnumerable<Customer> FilteredCustomers
        {
            get
            {
                string filterStr = customerFilter.ToLower().Trim();
                return Customers.Where(e => e.Guid.Contains(filterStr)
                    || e.Name.ToLower().Contains(filterStr)
                    || e.Phone.Contains(filterStr)
                    || e.Address.ToLower().Contains(filterStr));
            }
        }

        private string customerFilter = string.Empty;
        public string CustomerFilter
        {
            get { return customerFilter; }
            set
            {
                if (Set("CustomerFilter", ref customerFilter, value))
                    RaisePropertyChanged("FilteredCustomers");
            }
        }

        #endregion

        #region properties - items

        private List<Item> menuItems = new List<Item>();
        public List<Item> MenuItems
        {
            get { return menuItems; }
            set
            {
                if (Set("MenuItems", ref menuItems, value))
                    RaisePropertyChanged("FilteredMenuItems");
            }
        }

        private Item menuItem;
        public Item MenuItem
        {
            get { return menuItem; }
            set { Set("MenuItem", ref menuItem, value); }
        }

        public IEnumerable<Item> FilteredMenuItems
        {
            get
            {
                string filterStr = menuItemFilter.ToLower().Trim();
                return MenuItems.Where(i => i.Code.ToLower().Contains(filterStr));
            }
        }

        private string menuItemFilter = string.Empty;
        public string MenuItemFilter
        {
            get { return menuItemFilter; }
            set
            {
                if (Set("MenuItemFilter", ref menuItemFilter, value))
                    RaisePropertyChanged("FilteredMenuItems");
            }
        }

        #endregion

        #region properties - items in cart

        public List<Item> CartItems
        {
            get { return Entity.Items.OrderBy(i => i.Code).ToList(); }
        }

        private Item cartItem;
        public Item CartItem
        {
            get { return cartItem; }
            set { Set("CartItem", ref cartItem, value); }
        }

        public double TotalPrice
        {
            get { return Entity.TotalPrice; }
        }

        #endregion

        #region commands

        public RelayCommand CmdAddMenuItem { get; private set; }
        public RelayCommand CmdDeleteCartItem { get; private set; }
        public RelayCommand CmdConfirm { get; private set; }

        #endregion

        #region command implementations

        private bool CmdConfirmCanExecute()
        {
            // customer check
            if (IsCustomerNew && NewCustomer == null)
                return false;
            if (IsCustomerCur && CurCustomer == null)
                return false;
            if (!Entity.Items.Any())
                return false;
            return true;
        }

        private void CmdConfirmExecute()
        {
            // validation
            if ((IsCustomerCur && CurCustomer == null) || (IsCustomerNew && (string.IsNullOrWhiteSpace(NewCustomer.Name)
                    || string.IsNullOrWhiteSpace(NewCustomer.Address)
                    || string.IsNullOrWhiteSpace(NewCustomer.Phone))))
            {
                MessageBox.Show("Error! Missing customer information.");
                return;
            }

            // save new customer
            if (IsCustomerNew)
				cusRepository.Create(NewCustomer);

            Messenger.Default.Send(new CustomerStorageUpdatedEvent());

            // save order
            Entity.Customer = IsCustomerNew ? NewCustomer : CurCustomer;
			ordRepository.Create(Entity);

            Messenger.Default.Send(new OrderStorageUpdatedEvent());
            Messenger.Default.Send(new OrderDialogFinishedEvent());
            
            // print order
            PrinterHelper.Print(Entity);
        }

        private void CmdAddMenuItemExecute()
        {
            if (MenuItem == null)
                return;

            var existing = Entity.Items.FirstOrDefault(e => e.Code == MenuItem.Code);
            if (existing != null)
                existing.Quantity++;
            else
                Entity.Items.Add(new Item
                {
                    Guid = MenuItem.Guid,
                    Code = MenuItem.Code,
                    Name = MenuItem.Name,
                    Description = MenuItem.Description,
                    Price = MenuItem.Price,
                    Quantity = 1
                });

            RaisePropertyChanged("CartItems");
            RaisePropertyChanged("TotalPrice");
        }

        private void CmdDeleteCartItemExecute()
        {
            if (CartItem == null)
                return;

            var existing = Entity.Items.FirstOrDefault(e => e.Code == CartItem.Code);

            if (existing != null)
            {
                if (existing.Quantity > 1)
                    existing.Quantity--;
                else
                    Entity.Items.Remove(existing);
            }

            RaisePropertyChanged("CartItems");
            RaisePropertyChanged("TotalPrice");
        }

        private void CmdRefreshExecute()
        {
			Task.Run(() => Customers = cusRepository.RetrieveAll().ToList());
            Task.Run(() => MenuItems = itmRepository.RetrieveAll().ToList());
        }

        #endregion

        #region event handlers

        private void OnItemSelectedEvent(ItemSelectedEvent obj)
        {
            var existing = Entity.Items.FirstOrDefault(e => e.Code == obj.Entity.Code);

            if (existing != null)
                existing.Quantity++;
            else
                Entity.Items.Add(new Item
                {
                    Guid = obj.Entity.Guid,
                    Code = obj.Entity.Code,
                    Name = obj.Entity.Name,
                    Description = obj.Entity.Description,
                    Price = obj.Entity.Price,
                    Quantity = 1
                });

            RaisePropertyChanged("CartItems");
            RaisePropertyChanged("TotalPrice");
        }

        #endregion
    }
}
