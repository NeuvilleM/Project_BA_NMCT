using ProjectV3.View.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectV3.Model
{
    class LineUp: IDataErrorInfo
    {
        #region 'Fields'
        private String _id;

        public String ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private DateTime _date;
        [Required(ErrorMessage="Je moet verplicht een datum meegeven.")]
        //[Range(typeof(DateTime),Festival.start.ToShortDateString(),Festival.End.ToShortDateString(),ErrorMessage="Value for {0} must be between {1} and {2}")]
        [DateRangeValidator(ErrorMessage="Lineup moet plaatsvinden tijdens het festival")]
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
        private String _from;
        [Required(ErrorMessage="Geef startuur.")]
        [RegularExpression(@"^([0-1][0-9]|[2][0-3])u[0-5][0-9]$",ErrorMessage="HHuMM")]
        public String From
        {
            get { return _from; }
            set { _from = value; }
        }
        private String _until;
        [Required(ErrorMessage = "Geef einduur.")]
        [RegularExpression(@"^([0-1][0-9]|[2][0-3])u[0-5][0-9]$", ErrorMessage = "HHuMM")]
        public String Until
        {
            get { return _until; }
            set { _until = value; }
        }
        private Stage _stage;
        [Required(ErrorMessage="Je moet verplicht een stage meegeven.")]
        public Stage Stage
        {
            get { return _stage; }
            set { _stage = value; }
        }
        private Band _band;
        [Required(ErrorMessage="Je moet verplicht een band meegeven.")]
        public Band Band
        {
            get { return _band; }
            set { _band = value; }
        }
        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),
            null, true);
        }
        public override string ToString()
        {
            return Date.ToShortDateString() + ": " + Band.Name + " (" + From + "-" + Until + ")";
        }
        public string Error
        {
            get { return "Object Band is niet geldig."; }
        }

        public string this[string columnName]
        {
            get
            {
                try
                {
                    object value = this.GetType().GetProperty(columnName).GetValue(this);
                    Validator.ValidateProperty(value, new ValidationContext(this, null, null)
                    {
                        MemberName = columnName
                    });
                }
                catch (ValidationException ex)
                {
                    return ex.Message;
                }
                return String.Empty;
            }
        }
        #endregion
        #region 'Get data'
        public static ObservableCollection<LineUp> GetLineups(Model.Stage selectedstage)
        {
            try
            {
                ObservableCollection<LineUp> ocLineups = new ObservableCollection<LineUp>();
                if (selectedstage.ID != "0" && selectedstage.ID != null)
                {
                    string sql = "SELECT * FROM LineUp WHERE Stage = @StageId";
                    DbParameter SId = Database.AddParameter("@StageId", selectedstage.ID);
                    DbDataReader reader = Database.GetData(sql, SId);
                    while (reader.Read())
                    {
                        ocLineups.Add(MakeLineUp(reader, selectedstage));
                    }
                    reader.Close();
                }
                ObservableCollection<LineUp> ocLineupsSort = new ObservableCollection<LineUp>(from i in ocLineups orderby i.From select i);

                return ocLineupsSort;
            }
            catch
            {
                return new ObservableCollection<LineUp>();
            }
        }
        private static LineUp MakeLineUp(DbDataReader reader, Stage selectedS)
        {
            LineUp newlu = new LineUp();
            newlu.ID = reader["Id"].ToString();
            newlu.Stage = selectedS;
            newlu.Until = reader["Einde"].ToString();
            newlu.From = reader["Start"].ToString();
            newlu.Date = Convert.ToDateTime(reader["DateOfPlay"].ToString());
            newlu.Band = ZoekBand(reader["Artist"].ToString());
            return newlu;
        }
        private static Model.Band ZoekBand(string id)
        {
            ObservableCollection<Genre> g = Genre.GetGenres();
            string sql = "SELECT * FROM Artist WHERE Id = @ID";
            DbParameter Id = Database.AddParameter("@ID", id);
            DbDataReader reader = Database.GetData(sql, Id);
            Band b = new Band();
            reader.Read();
            b = Band.MaakArtist(reader, g);
            reader.Close();
            return b;
        }
        #endregion
        #region 'Save data'
        public void SaveLineUp() 
        {
            string dateSQL = MakeDateForSQL(this.Date);
            int iAffectedRows = 0;
            DbParameter Artist = Database.AddParameter("@Artist", Convert.ToInt32(this.Band.ID));
            DbParameter date = Database.AddParameter("@Date", dateSQL);
            DbParameter from = Database.AddParameter("@from", this.From);
            DbParameter until = Database.AddParameter("@until", this.Until);
            DbParameter stage = Database.AddParameter("@stage", Convert.ToInt32(this.Stage.ID));
            if (this.ID != null && this.ID != "" && this.ID != "0")
            {
                DbParameter id = Database.AddParameter("@ID", this.ID);
                string sql = "UPDATE LineUp SET Stage = @stage, Artist = @Artist, DateOfPlay = @Date, Start = @from, Einde = @until WHERE Id = @ID";
                iAffectedRows += Database.ModifyData(sql, stage, Artist, date, from, until, id);
            }
            else
            {
                // string sql = "INSERT INTO LineUp (Stage,Band,DateOfPlay,Start,End) VALUES (@stage,@band,@Date,@from,@until)";
                // iAffectedRows += Database.ModifyData(sql, stage, band, date, from, until);
                string sql2 = "INSERT INTO LineUp (Stage,Artist,DateOfPlay,Start,Einde) VALUES (@stage,@Artist, @Date,@from,@until)";
               // string sql3 = "INSERT INTO LineUp (Stage,Artist,DateOfPlay,Start,End) VALUES (@stage,@band,@Date,@from,@from,@until)";
                iAffectedRows += Database.ModifyData(sql2, stage, Artist, date, from, until);
            }
            Console.WriteLine("Er zijn " + iAffectedRows + " rijen gewijzigt in LineUp");
        }

        private string MakeDateForSQL(DateTime dateTime)
        {
            string date = Convert.ToString(dateTime.Year) + "/";
            date += Convert.ToString(dateTime.Month)+"/";
            date += Convert.ToString(dateTime.Day);
            return date;
        }
        #endregion
        #region 'Delete data'
        public void DeleteLineUp()
        {
            int iAffectedRows = 0;
            string sql = "DELETE FROM LineUp WHERE Id = @ID";
            DbParameter id = Database.AddParameter("@ID", this.ID);
            iAffectedRows += Database.ModifyData(sql, id);
            Console.WriteLine("Er zijn " + iAffectedRows + " rijen verwijdert uit LineUp");
        
        }
        #endregion

    }
}
