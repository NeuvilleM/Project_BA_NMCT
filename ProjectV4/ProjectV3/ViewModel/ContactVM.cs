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
    class ContactVM : ObservableObject, IPage
    {
        #region 'Fields en constructor'
        public ContactVM() {
            SelectedContact = new ContactPerson() {ID = "" };
            _contacts = ContactPerson.GetContacts();
            ContactsForListbox = Contacts;
            SearchCriteria = "";
            _typesContactVM = ContactpersonType.GetContactTypes();
            NewExecute();
        }
        public string Name
        {
            get { return "Contact"; }
        }
        private ObservableCollection<ContactPerson> _contacts;
        private ObservableCollection<ContactPerson> _contactsForListbox;

        public ObservableCollection<ContactPerson> ContactsForListbox
        {
            get { return _contactsForListbox; }
            set { _contactsForListbox = value;
            OnPropertyChanged("ContactsForListbox");
            }
        }
        
        public ObservableCollection<ContactPerson> Contacts
        {
            get { return _contacts; }
            set { _contacts = value;
            OnPropertyChanged("Contact");
            }
        }
        private String _searchCriteria;

        public String SearchCriteria
        {
            get { return _searchCriteria; }
            set { _searchCriteria = value;
            ZoekFunctie();
            OnPropertyChanged("SearchCriteria");
            }
        }
        
        private ContactPerson _selectedContact;

        public ContactPerson SelectedContact
        {
            get { return _selectedContact; }
            set { _selectedContact = value;
            OnPropertyChanged("SelectedContact");
            }
        }
        private ObservableCollection<ContactpersonType> _typesContactVM;

        public ObservableCollection<ContactpersonType> TypesContactVM
        {
            get { return _typesContactVM; }
            set { _typesContactVM = value;
            OnPropertyChanged("TypesContactVM");
            }
        }
        #endregion
        #region 'Object specifiek methodes'
        public void ZoekFunctie() {
            if (SearchCriteria != "")
            {
                ContactsForListbox = Contacts;
                ObservableCollection<ContactPerson> filter1 = new ObservableCollection<ContactPerson>();
                ObservableCollection<ContactPerson> filter2 = new ObservableCollection<ContactPerson>();
                ObservableCollection<ContactPerson> filter3 = new ObservableCollection<ContactPerson>();
                    foreach (ContactPerson cp in ContactsForListbox)
                    {
                        string sName = cp.Name.ToLower();
                        string sCompany = cp.Company.ToLower();
                        string sJobrole = cp.JobRole.Name.ToLower();
                        string sCriteria = SearchCriteria.ToLower();
                        if (sName.Contains(sCriteria))
                            filter1.Add(cp);
                        if (sCompany.Contains(sCriteria))
                            filter2.Add(cp);
                        if (sJobrole.Contains(sCriteria))
                            filter3.Add(cp);
                    }
                    foreach (ContactPerson cp in filter2)
                    {
                        if (filter1.IndexOf(cp) > -1)
                            filter1.Remove(cp);
                    }
                    foreach (ContactPerson cp in filter1)
                    {
                        filter2.Add(cp);
                    }
                    foreach (ContactPerson cp in filter3)
                    {
                        if (filter2.IndexOf(cp) < 0)
                            filter2.Add(cp);
                    }
                    ContactsForListbox = filter2;
                    
                
            }
            else
            {
                ContactsForListbox = Contacts;
            }
        
        }
        #endregion
        #region 'commandsContactPerson'
        #region 'save-command'
        private ICommand _SaveContacts;

        public ICommand SaveContacts
        {
            get { return new RelayCommand(SaveExecute, CanExecuteSaveContacten); }
            private set { _SaveContacts= value; }
        }
        private bool CanExecuteSaveContacten()
        {
            if (SelectedContact == null) { NewExecute(); return false; }
            if (SelectedContact.Name == "Naam?")
                return false;
            if (SelectedContact.JobRole == null || SelectedContact.JobRole.ID == "" || SelectedContact.JobRole.ID == "0") return false;
            
            if (SelectedContact.IsValid()) return true;
            else return false;
        }

        public void SaveExecute()
        {
            SelectedContact.SaveContact();
            NewExecute();
            
        }
        #endregion
        #region 'new-command'
        private ICommand _NewContact;

        public ICommand NewContact
        {
            get { return new RelayCommand(NewExecute, CanExecuteNewContact); }
            private set { _NewContact = value; }
        }
        private bool CanExecuteNewContact()
        {
            return true;
        }

        public void NewExecute()
        {
            ContactPerson cp = new ContactPerson();
            cp.Name = "Naam?";
            cp.Phone = "Telefoon?";
            cp.Cellphone = "GSM-nummer";
            cp.Company = "Bedrijf?";
            cp.Email = "Email?";
            cp.City = "Stad?";
            cp.JobRole = new ContactpersonType() {ID = "1" };
            SelectedContact = cp;
            Contacts = ContactPerson.GetContacts();
            ContactsForListbox = Contacts;
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteContact;

        public ICommand DeleteContact
        {
            get { return new RelayCommand(DeleteExecute, CanExecuteDeleteContact); }
            private set { _DeleteContact = value; }
        }
        private bool CanExecuteDeleteContact()
        {
            return true;
        }

        public void DeleteExecute()
        {
            SelectedContact.DeleteContact();
            NewExecute();  
        }
        #endregion
        #endregion
        
    }
}
