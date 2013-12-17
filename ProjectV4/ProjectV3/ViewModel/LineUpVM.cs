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
            Festival = Festival.GetFestival();
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
            if (!IsTimePlacingCorrect(SelectedLineUp))
                return false;
            if (SelectedLineUp.IsValid()) return true;
            else return false;
        }

        private bool IsTimePlacingCorrect(LineUp SelectedLineUp)
        {
            
            // linups komen gesorteerd binnen op startuur
            // STAP 1: controleren of einde niet voor start ligt
            if (!IsStartBeforeEnd()) return false; 
            // er is nu al gecontroleerd geweest of de uren in uren wel effectief uren zijn
            // dit voorkomt dat we dit opnieuw moeten controleren
            // STAP 2: controleren of start niet tijdens speeluren van anderen band ligt
            // STAP 3: controleren of einde niet tijdens speeluren van andere band ligt
            // STAP 4: controleren of einde voor start volgende band ligt
            return true;
        }

        
        private int[] getStartAndEnd(LineUp tezoekenlinup)
        {
            int[] uren = new int[4];
            int iUurStart;
            int iMinStart;
            string sUurStart = tezoekenlinup.From.Substring(0, 2);
            string sMinStart = tezoekenlinup.From.Substring(3, 2);
            if (Int32.TryParse(sUurStart, out iUurStart) && Int32.TryParse(sMinStart, out iMinStart))
            {
                if (iUurStart < 24 && iUurStart >= 0) uren[0] = iUurStart;
                else uren[0] = -1;
                if (iMinStart < 60 && iMinStart >= 0) uren[1] = iMinStart;
                else uren[1] = -1;
            }
            int iUurEnd;
            int iMinEnd;
            string sUurEnd = tezoekenlinup.Until.Substring(0, 2);
            string sMinEnd = tezoekenlinup.Until.Substring(3, 2);
            if (Int32.TryParse(sUurEnd, out iUurEnd) && Int32.TryParse(sMinEnd, out iMinEnd))
            {
                if (iUurEnd < 24 && iUurEnd >= 0) uren[2] = iUurEnd;
                else uren[2] = -1;
                if (iMinEnd < 60 && iMinEnd >= 0) uren[3] = iMinEnd;
                else uren[3] = -1;
            }
            return uren;
        }
        private bool IsStartBeforeEnd()
        {
            if (SelectedLineUp.From == null || SelectedLineUp.Until == null) return false;
        if (SelectedLineUp.From.Length != 5 || SelectedLineUp.Until.Length != 5) return false;
        int [] uren = getStartAndEnd(SelectedLineUp);
        foreach (int i in uren) { if (i < 0)return false; }
        // nu we zeker zijn dat alle waarden in uren effectief een tijdswaarde is kunnen
        // we verder controleren uren[uurstart, minstart, uurend, minend]
        if (uren[2] < uren[0]) return false;
        if (uren[2] == uren[0])
        {
            if (uren[3] < uren[1]) return false;
        }
        return true;
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
