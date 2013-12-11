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
    class Stage: IDataErrorInfo
    {
        private String _id;

        public String ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private String _name;
        [Required(ErrorMessage="Je moet een naam invullen.")]
        [StringLength(45,MinimumLength=2, ErrorMessage="Je moet een lente ingeven tussen 2 en 45 karakters.")]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),
            null, true);
        }
        public override string ToString()
        {
            return Name;
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
        #region 'Get data'
        public static ObservableCollection<Stage> GetStages()
        {
            ObservableCollection<Stage> stages = new ObservableCollection<Stage>();
            string sql = "SELECT * FROM Stages";
            DbDataReader reader = Database.GetData(sql);
            while (reader.Read())
            {
                stages.Add(MakeStage(reader));
            }
            ObservableCollection<Stage> stagesSort = new ObservableCollection<Stage>(from i in stages orderby i.Name select i);
            reader.Close();
            return stagesSort;
        
        }
        private static Stage MakeStage(DbDataReader reader)
        {
            Stage s = new Stage();
            s.Name = reader["StageName"].ToString();
            s.ID = reader["Id"].ToString();
            return s;
        }
        #endregion
        #region 'save data'
        public void SaveStage() 
        {
            int iAffectedRows = 0;
            DbParameter Name = Database.AddParameter("@Name", this.Name);
            if (this.ID != null && this.ID != "")
            {
                DbParameter id = Database.AddParameter("@ID", this.ID);
                string sql = "UPDATE Stages SET StageName = @Name WHERE Id= @ID";
                iAffectedRows += Database.ModifyData(sql, Name, id);
            }
            else 
            {
                string sql = "INSERT INTO Stages (StageName) VALUES (@Name)";
                iAffectedRows += Database.ModifyData(sql, Name);
            }
            Console.WriteLine("Er zijn " + iAffectedRows + " rijen gewijzigt in Stages");       
        }
        #endregion
        #region 'Delete stage'
        public void DeleteStage() 
        {
            int iAffectedRows = 0;
            DbParameter id = Database.AddParameter("@ID", this.ID);
            string sql = "DELETE FROM Stages WHERE Id = @ID";
            iAffectedRows += Database.ModifyData(sql, id);
            Console.WriteLine("Er zijn " + iAffectedRows + " rijen verwijdert uit Stages");
        }
        #endregion
    }
}
