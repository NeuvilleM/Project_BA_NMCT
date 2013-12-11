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
    class LineUpVM : ObservableObject, IPage
    {
        #region 'Fields en constructors'
        public LineUpVM()
        {
            _stages = Stage.GetStages();
            Festival = new Festival();
            Festival.GetFestival();
            Artists = Band.GetArtists();
            _selectedStage = new Stage();
        }
        public string Name
        {
            get { return "LineUp"; }
        }
        private ObservableCollection<Stage> _stages;

        public ObservableCollection<Stage> Stages
        {
            get { return _stages; }
            set { _stages = value;
            OnPropertyChanged("Stages");
            }
        }
        private ObservableCollection<LineUp> _lineUps;
        private Festival _festival;

        public Festival Festival
        {
            get { return _festival; }
            set { _festival = value;
            OnPropertyChanged("Festival");
            }
        }

        private ObservableCollection<Band> _artists;

        public ObservableCollection<Band> Artists
        {
            get { return _artists; }
            set { _artists = value;
            OnPropertyChanged("Artists");
            }
        }
        public ObservableCollection<LineUp> LineUps
        {
            get { return _lineUps; }
            set { _lineUps = value;
            OnPropertyChanged("LineUps");
            }
        }
        private LineUp _selectedLineUp;

        public LineUp SelectedLineUp
        {
            get { return _selectedLineUp; }
            set { _selectedLineUp = value;
            OnPropertyChanged("SelectedLineUp");
            }
        }
        
        private Stage _selectedStage;

        public Stage SelectedStage
        {
            get { return _selectedStage; }
            set { _selectedStage = value;
            LineUps = LineUp.GetLineups(SelectedStage);
            OnPropertyChanged("LineUps");
            OnPropertyChanged("SelectedStage");
            }
        }
        #endregion
        #region 'commandsStage'
        #region 'save-command'
        private ICommand _SaveStage;

        public ICommand SaveStage
        {
            get { return new RelayCommand(SaveExecuteStage, CanExecuteSaveStage); }
            private set { _SaveStage = value; }
        }
        private bool CanExecuteSaveStage()
        {
            if (SelectedStage == null)
            {
                NewExecuteStage();
                return false;
            }
            if (SelectedStage.Name != "Name?")
                return true;
            else return false;
        }

        public void SaveExecuteStage()
        {
            SelectedStage.SaveStage();
            Stages = Stage.GetStages();
            NewExecuteStage();
        }
        #endregion
        #region 'new-command'
        private ICommand _NewStage;

        public ICommand NewStage
        {
            get { return new RelayCommand(NewExecuteStage, CanExecuteNewStage); }
            private set { _NewStage = value; }
        }
        private bool CanExecuteNewStage()
        {
            return true;
        }

        public void NewExecuteStage()
        {
            Stage s = new Stage() { Name = "Stage?" };
            SelectedStage = s;
            Stages = Stage.GetStages();
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteStage;

        public ICommand DeleteStage
        {
            get { return new RelayCommand(DeleteExecuteStage, CanExecuteDeleteStage); }
            private set { _DeleteStage = value; }
        }
        private bool CanExecuteDeleteStage()
        {
            if (SelectedStage != null && SelectedStage.Name != "Stage?" && SelectedStage.IsValid()) { return true; }
            return false;
        }
        public void DeleteExecuteStage()
        {
            SelectedStage.DeleteStage();
            NewExecuteStage();
        }
        #endregion
        #endregion
        #region 'commandsLineUp'
        #region 'save-command'
        private ICommand _SaveLineUp;

        public ICommand SaveLineUp
        {
            get { return new RelayCommand(SaveExecuteLineUp, CanExecuteSaveLineUp); }
            private set { _SaveLineUp = value; }
        }
        private bool CanExecuteSaveLineUp()
        {
            if (SelectedLineUp == null)
            {
                NewExecuteLineUp();
                return false;
            }
            if (SelectedLineUp.Band == null)
                return false;
            if (SelectedLineUp.IsValid()) return true;
            else return false;
        }

        public void SaveExecuteLineUp()
        {
            SelectedLineUp.SaveLineUp();
            NewExecuteLineUp();
        }
        #endregion
        #region 'new-command'
        private ICommand _NewLineUp;

        public ICommand NewLineUp
        {
            get { return new RelayCommand(NewExecuteLineUp, CanExecuteNewLineUp); }
            private set { _NewLineUp = value; }
        }
        private bool CanExecuteNewLineUp()
        {
            return true;
        }

        public void NewExecuteLineUp()
        {
            LineUp lu = new LineUp() { ID = "0" };
            lu.Stage = SelectedStage;
            SelectedLineUp = lu;
            LineUps = LineUp.GetLineups(SelectedStage);
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteLineUp;

        public ICommand DeleteLineUp
        {
            get { return new RelayCommand(DeleteExecuteLineUp, CanExecuteDeleteLineUp); }
            private set { _DeleteLineUp = value; }
        }
        private bool CanExecuteDeleteLineUp()
        {
            if (SelectedLineUp != null && SelectedLineUp.ID != null && SelectedLineUp.ID != "0") { return true; }
            return false;
        }
        public void DeleteExecuteLineUp()
        {
            SelectedLineUp.DeleteLineUp();
            NewExecuteLineUp();
        }
        #endregion
        #endregion

    }
}
