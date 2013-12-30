using System;
using ProjectV3.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel.DataAnnotations;
/* Wat moet artiest-pagina kunnen?
 * 1. weergeven van data uit database (testen met andere databron)
 * 1.1 enkel de listbox mag geenabeld zijn, de rest is gedisabeld zolang er niet op edit of op new gedrukt is.
 * 2. data aanpassen door op edit te klikken
 * 2.1 listbox disabelen en alle andere niet noodzakelijke controls disabelen
 * 3. Data opnieuw wegschrijven
 */
/* Validatie gegegevens
 * 1. Voeg validatieregels toe in model, implementeer interface
 * 2. update bindings
 * 3. voeg method IsValid toe aan model
 * 4. update canSaveExecute
 */
namespace ProjectV3.ViewModel
{
    class ArtistVM : ObservableObject, IPage
    {
        #region 'Fields and constructor'
       
       private ObservableCollection<Band> _bands;

       public ObservableCollection<Band> Bands
       {
           get { return _bands; }
           set { _bands = value;
           OnPropertyChanged("Bands");
           }
       }
       
       public ArtistVM()
        {
            _geselecteerdeArtiest = new Band();
            _bands = Band.GetArtists();
            NewExecute();
            _selectedGenre = new Genre();
            _genresArtiestVM = Genre.GetGenres();
        }
       
        public string Name
        {
            get { return "Artiest"; }
        }
        private Band _geselecteerdeArtiest;
        public Band GeselecteerdeArtiest
        {
            get
            {
                return _geselecteerdeArtiest;
            }
            set
            {
                _geselecteerdeArtiest = value;
                Console.WriteLine("Er is een band geselecteerd: " + _geselecteerdeArtiest);
                OnPropertyChanged("GeselecteerdeArtiest");
            }
        }
        private Genre _selectedGenre;
        public Genre SelectedGenre
        {
            get { return _selectedGenre; }
            set
            {
                _selectedGenre = value;
                OnPropertyChanged("SelectedGenre");
            }
        }
        private ObservableCollection<Genre> _genresArtiestVM;
        public ObservableCollection<Genre> GenresArtiestVM
        {
            get { return _genresArtiestVM; }
            set { _genresArtiestVM = value;
            OnPropertyChanged("GenresArtiestVM");
            }
        }
        #endregion


        #region 'commands'
        #region 'save-command'
        private ICommand _SaveBands;

        public ICommand SaveBands
        {
            get { return new RelayCommand(SaveExecute, CanExecuteSaveArtiesten); }
            private set { _SaveBands = value; }
        }
        private bool CanExecuteSaveArtiesten() {
            
            if (GeselecteerdeArtiest == null)
            {
                NewExecute();
                return false; }
            if (GeselecteerdeArtiest.Name == "Naam?")
            {
              return false;
            }
            if (GeselecteerdeArtiest.IsValid()) return true;
            else return false;
        }
        
        public void SaveExecute()
        {
            GeselecteerdeArtiest.SaveArtist();
            NewExecute();           
        }
        #endregion
        #region 'new-command'
        private ICommand _NewBand;

        public ICommand NewBands
        {
            get { return new RelayCommand(NewExecute, CanExecuteNewArtiesten); }
            private set { _NewBand = value; }
        }
        private bool CanExecuteNewArtiesten()
        {
            return true;
        }

        public void NewExecute()
        {
            Band nieuweband = new Band();
            nieuweband.Name = "Naam?";
            nieuweband.Description="Beschrijving?";
            nieuweband.Twitter="???";
            nieuweband.Facebook = "???";
            nieuweband.Picture = "Geen afbeelding";
            nieuweband.Genres = new ObservableCollection<Genre>();
            GeselecteerdeArtiest = nieuweband;
            Bands = Band.GetArtists();
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteBands;

        public ICommand DeleteBands
        {
            get { return new RelayCommand(DeleteExecute, CanExecuteDeleteArtiesten); }
            private set { _DeleteBands = value; }
        }
        private bool CanExecuteDeleteArtiesten()
        {
            return true;
        }
        public void DeleteExecute()
        {
            GeselecteerdeArtiest.DeleteArtist();
            NewExecute();
            
            Console.WriteLine("Delete Artist completed");
        }
        #endregion
        #endregion
        #region 'commandsArtiestGenres'
        #region 'save-command'
        private ICommand _SaveAG;

        public ICommand SaveAG
        {
            get { return new RelayCommand(SaveExecuteAG, CanExecuteSaveAG); }
            private set { _SaveAG = value; }
        }
        private bool CanExecuteSaveAG()
        {
            if (SelectedGenre != null && SelectedGenre.Name != null )
                return true;
            else return false;
        }

        public void SaveExecuteAG()
        {
            int i = -1;
            for (int z = 0; z < GeselecteerdeArtiest.Genres.Count(); z++)
            {
                if (GeselecteerdeArtiest.Genres[z].Name.Equals(SelectedGenre.Name))
                {
                    i = z;
                    break;
                }
            }
            if (i < 0)
            {
                GeselecteerdeArtiest.Genres.Add(SelectedGenre);

            }
            else
            {
                GeselecteerdeArtiest.Genres[i] = SelectedGenre;
            }
            
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteAG;

        public ICommand DeleteAG
        {
            get { return new RelayCommand(DeleteExecuteAG, CanExecuteDeleteAG); }
            private set { _DeleteAG = value; }
        }
        private bool CanExecuteDeleteAG()
        {
            return true;
        }

        public void DeleteExecuteAG()
        {

            int i = -1;
            if (SelectedGenre != null && SelectedGenre.Name != null)
            {
                foreach (Genre b in GeselecteerdeArtiest.Genres)
                {
                    i++;
                    if (b.Name.Equals(SelectedGenre.Name))
                        break;

                }
            }
            if (i < 0)
            {
            }
            
            else
            {
                
                GeselecteerdeArtiest.Genres.RemoveAt(i);
            }
            foreach (Genre s in GeselecteerdeArtiest.Genres) { Console.WriteLine(s.ToString()); }
        }
        #endregion
        #endregion

    }
}
