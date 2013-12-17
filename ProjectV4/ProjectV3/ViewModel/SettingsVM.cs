using GalaSoft.MvvmLight.Command;
using ProjectV3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace ProjectV3.ViewModel
{
    class SettingsVM : ObservableObject, IPage
    {
        #region 'Fields en constructor'
        public SettingsVM()
        {
            _selectedCPT = new ContactpersonType();
            NewExecuteGenre();
            NewExecuteJobrole();
            _festival = Festival.GetFestival();
            _contactpersontypes = ContactpersonType.GetContactTypes();
            Genres = Genre.GetGenres();
            
        }
        public string Name
        {
            get { return "Settings"; }
        }
        private Festival _festival;

        public Festival Festival  
        {
            get { return _festival; }
            set { _festival = value;
            OnPropertyChanged("Festival");
            }
        }
        private ContactpersonType _selectedCPT;

        public ContactpersonType SelectedCPT
        {
            get { return _selectedCPT; }
            set { _selectedCPT = value;
            OnPropertyChanged("SelectedCPT");
            }
        }
        private Genre _selectedGenre;

        public Genre SelectedGenre
        {
            get { return _selectedGenre; }
            set { _selectedGenre = value;
            OnPropertyChanged("SelectedGenre");
            }
        }
        private ObservableCollection<Genre> _genres;

        public ObservableCollection<Genre> Genres
        {
            get { return _genres; }
            set { _genres = value;
            OnPropertyChanged("Genres");
            }
        }
        private ObservableCollection<ContactpersonType> _contactpersontypes;

        public ObservableCollection<ContactpersonType> ContactPersonTypes
        {
            get { return _contactpersontypes; }
            set
            {
                _contactpersontypes = value;
                OnPropertyChanged("ContactPersonTypes");
            }
        }
        #endregion
        #region 'commandsCPT'
        #region 'save-command'
        private ICommand _SaveCPT;

        public ICommand SaveCPT
        {
            get { return new RelayCommand(SaveExecute, CanExecuteSaveCPT); }
            private set { _SaveCPT = value; }
        }
        private bool CanExecuteSaveCPT()
        {
            if (SelectedCPT == null)
            {
                NewExecuteJobrole();
                return false;
            }
            if (SelectedCPT.Name == "Jobrole?")
                return false;
            if (SelectedCPT.IsValid()) return true;
            else return false;
        }

        public void SaveExecute()
        {
            SelectedCPT.SaveJobrole();
            NewExecuteJobrole();
            //foreach (ContactPerson s in Contacts.Contacts) { Console.WriteLine(s.ToString()); }

        }
        #endregion
        #region 'new-command'
        private ICommand _NewCPT;

        public ICommand NewCPT
        {
            get { return new RelayCommand(NewExecuteJobrole, CanExecuteNewContact); }
            private set { _NewCPT = value; }
        }
        private bool CanExecuteNewContact()
        {
            return true;
        }

        public void NewExecuteJobrole()
        {
            SelectedCPT = new ContactpersonType() { Name = "Jobrole?" };
            ContactPersonTypes = ContactpersonType.GetContactTypes();
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteCPT;

        public ICommand DeleteCPT
        {
            get { return new RelayCommand(DeleteExecute, CanExecuteDeleteCPT); }
            private set { _DeleteCPT = value; }
        }
        private bool CanExecuteDeleteCPT()
        {
            if (SelectedCPT == null) { return false; }
            if (SelectedCPT.Name == "Jobrole?") { return false; }
            return true;
        }

        public void DeleteExecute()
        {
            SelectedCPT.DeleteJobrole();
            NewExecuteJobrole();
        }
        #endregion
        #endregion
        #region'commandsGenre'
        #region 'save-command'
        private ICommand _SaveGenre;

        public ICommand SaveGenre
        {
            get { return new RelayCommand(SaveExecuteGenre, CanExecuteSaveGenre); }
            private set { _SaveGenre = value; }
        }
        private bool CanExecuteSaveGenre()
        {
            if (SelectedGenre == null) {
                NewExecuteGenre();
                return false;
            }
            if (SelectedGenre.Name == "Name?") return false;
            if(SelectedGenre.IsValid())
                return true;
            else return false;
        }
        public void SaveExecuteGenre()
        {
            SelectedGenre.SaveGenre();
            NewExecuteGenre();
        }
        #endregion
        #region 'new-command'
        private ICommand _NewGenre;

        public ICommand NewGenre
        {
            get { return new RelayCommand(NewExecuteGenre, CanExecuteNewGenre); }
            private set { _NewGenre = value; }
        }
        private bool CanExecuteNewGenre()
        {
            return true;
        }

        public void NewExecuteGenre()
        {
            Genre g = new Genre();
            g.Name = "Name?";
            SelectedGenre = g;
            Genres = Genre.GetGenres();
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteGenre;

        public ICommand DeleteGenre
        {
            get { return new RelayCommand(DeleteExecuteGenre, CanExecuteDeleteGenre); }
            private set { _DeleteGenre = value; }
        }
        private bool CanExecuteDeleteGenre()
        {
            if (SelectedGenre.ID == "15") return false;
            if (SelectedGenre.Name == "Name?") { return false; }
            return true;
        }

        public void DeleteExecuteGenre()
        {
            SelectedGenre.DeleteGenre();
            NewExecuteGenre();
        }
        #endregion
        #endregion
        #region 'SaveCommandFestival' 
        #region 'save-command'
        private ICommand _SaveFestival;

        public ICommand SaveFestival
        {
            get { return new RelayCommand(SaveExecuteFestival, CanExecuteSaveFestival); }
            private set { _SaveFestival = value; }
        }
        private bool CanExecuteSaveFestival()
        {
            if (Festival == null)
            {
                return false;
            }
            if (Festival.IsValid()) return true;
            else return false;
        }
        public void SaveExecuteFestival()
        {
            Festival.SaveFestival();
            Festival = Festival.GetFestival();
        }
        #endregion
        #endregion

    }
}
