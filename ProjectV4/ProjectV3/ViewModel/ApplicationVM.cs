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
    // deze klasse is het hart van je applicatie
    class ApplicationVM : ObservableObject
    {
        public ApplicationVM()
        {
            
            _pages = new ObservableCollection<IPage>();
            // hieronder hoeven we al een 1ste ipage opbject toe
            // bij nieuwe Pages moet deze lijst aangevuld worden met telkens
            // de bijhorende viewmodel-klasse
            _pages.Add(new HomePageVM());
            _pages.Add(new ArtistVM());
            _pages.Add(new ContactVM());
            _pages.Add(new LineUpVM());
           
            _pages.Add(new TicketingVM());
            _pages.Add(new SettingsVM());

            // default zetten we de CurrentPage in op de eerste IPage (hier HomePage)
            _currentPage = Pages[0];
            Festival = Festival.GetFestival();
        }
        private IPage _currentPage;
        public IPage CurrentPage
        {
            get
            {
                return _currentPage; // huidige page( dat nu getoond wordt)
            }
            set
            {
                _currentPage = value;
                // ik maak aan de buitenwereld bekent dat de property gewijzigt is. Eventuele controls die
                // eraan gebonden zijn, worden nu aangepast.
                OnPropertyChanged("CurrentPage");
            }

        }
        public Festival Festival { get; set; }
        private ObservableCollection<IPage> _pages;
        public ObservableCollection<IPage> Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;
                OnPropertyChanged("Pages");
            }
        }
        
        

        // onderstaande komt uit de cursus
        // deze 2 methodes worden gebruikt door de buttons (op mainwindow.xaml)
        // en kan het juiste commando activeren. Hier is dat het wisselen van Page
        public ICommand ChangePageCommand
        {
            get { return new RelayCommand<IPage>(ChangePage); }
        }
        private void ChangePage(IPage page)
        {
            CurrentPage = page;
        }

    }
}
