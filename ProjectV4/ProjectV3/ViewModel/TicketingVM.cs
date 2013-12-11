using GalaSoft.MvvmLight.Command;
using ProjectV3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectV3.ViewModel
{
    class TicketingVM : ObservableObject, IPage
    {
        #region 'Fields en constructor'
        public TicketingVM()
        {
            Types = TicketType.GetTicketTypes();
            OrderedTickets = Ticket.GetTickets();
            SelectedType = new TicketType() {Name = "Enter Type?", AvailableTickets = 0, Price= 0 };
            SelectedTicket = new Ticket() { HolderLast = "Last Name?", Ticketholder = "First Name?", TicketholderEmail = "Email" };
        }
        public string Name
        {
            get { return "Ticketing"; }
        }
        private ObservableCollection<Ticket> _orderedTickets;

        public ObservableCollection<Ticket> OrderedTickets
        {
            get { return _orderedTickets; }
            set { _orderedTickets = value;
            OnPropertyChanged("OrderedTickets");
            }
        }

        private ObservableCollection<TicketType> _types;

        public ObservableCollection<TicketType> Types
        {
            get { return _types; }
            set { _types = value;
            OnPropertyChanged("Types");
            }
        }
        private TicketType _selectedType;

        public TicketType SelectedType
        {
            get { return _selectedType; }
            set { _selectedType = value;
            OnPropertyChanged("SelectedType");
            }
        }
        private Ticket _selectedTicket;

        public Ticket SelectedTicket
        {
            get { return _selectedTicket; }
            set { _selectedTicket = value;
            OnPropertyChanged("SelectedTicket");
            }
        }
        
        #endregion
        #region 'Commands For Type'
        #region 'save-command'
        private ICommand _SaveType;

        public ICommand SaveType
        {
            get { return new RelayCommand(SaveExecute, CanExecuteSaveTypes); }
            private set { _SaveType = value; }
        }
        private bool CanExecuteSaveTypes()
        {
            if (SelectedType == null)
            {
                NewExecuteType();
                return false;
            }
            if (SelectedType.Name == "Enter Type?")
                return false;
            if (SelectedType.IsValid()) return true;
            else return false;
        }

        public void SaveExecute()
        {
            SelectedType.SaveType();
            NewExecuteType();         
        }
        #endregion
        #region 'new-command'
        private ICommand _NewType;

        public ICommand NewType
        {
            get { return new RelayCommand(NewExecuteType, CanExecuteNewType); }
            private set { _NewType = value; }
        }
        private bool CanExecuteNewType()
        {
            return true;
        }

        public void NewExecuteType()
        {
            SelectedType = new TicketType() { Name = "Enter Type?", AvailableTickets = 0, Price = 0 };
            Types = TicketType.GetTicketTypes();
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteType;

        public ICommand DeleteType
        {
            get { return new RelayCommand(DeleteExecuteType, CanExecuteDeleteType); }
            private set { _DeleteType = value; }
        }
        private bool CanExecuteDeleteType()
        {
            if (SelectedType.ID == null) { return false; }
            if (SelectedType.ID == "") { return false; }
            return true;
        }
        public void DeleteExecuteType()
        {
            SelectedType.DeleteType();
            SelectedType = new TicketType() { Name = "Enter Type?", AvailableTickets = 0, Price = 0 };
            NewExecuteType();
            
        }
        #endregion
        #endregion
        #region 'Commands For Ticket'
        #region 'save-command'
        private ICommand _SaveTicket;

        public ICommand SaveTicket
        {
            get { return new RelayCommand(SaveExecuteTicket, CanExecuteSaveTicket); }
            private set { _SaveTicket = value; }
        }
        private bool CanExecuteSaveTicket()
        {
            if (SelectedTicket == null)
            {
                NewExecuteTicket();
                return false;
            }
            if (SelectedTicket.Ticketholder == "First Name?")
                return false;
            if (SelectedTicket.IsValid()) return true;
            else return false;
        }

        public void SaveExecuteTicket()
        {
            SelectedTicket.SaveTicket();
            NewExecuteTicket();
        }
        #endregion
        #region 'new-command'
        private ICommand _NewTicket;

        public ICommand NewTicket
        {
            get { return new RelayCommand(NewExecuteTicket, CanExecuteNewTicket); }
            private set { _NewTicket = value; }
        }
        private bool CanExecuteNewTicket()
        {
            return true;
        }

        public void NewExecuteTicket()
        {
            SelectedTicket = new Ticket() { HolderLast = "Last Name?", Ticketholder = "First Name?", TicketholderEmail = "Email" };
            OrderedTickets = Ticket.GetTickets();
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteTicket;

        public ICommand DeleteTicket
        {
            get { return new RelayCommand(DeleteExecuteTicket, CanExecuteDeleteTicket); }
            private set { _DeleteTicket = value; }
        }
        private bool CanExecuteDeleteTicket()
        {
            if (SelectedTicket.ID == null) { return false; }
            if (SelectedTicket.ID == "") { return false; }
            return true;
        }
        public void DeleteExecuteTicket()
        {
            SelectedTicket.DeleteTicket();
            NewExecuteTicket();

        }
        #endregion
        #endregion


    }
}
