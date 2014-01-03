using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Xml;

namespace ProjectV3.Model
{

    class ContactpersonType: IDataErrorInfo
    {
        #region 'Fields'
        private String _id;

        public String ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private String _name;
        [Required(ErrorMessage="De naam is verplicht mee te geven.")]
        [StringLength(45, MinimumLength=2, ErrorMessage="De lengte moet tussen de 2 en 45 karakters zijn.")]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public override string ToString()
        {
            return Name;
        }
        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),
            null, true);
        }
        public string Error
        {
            get { return "Object ContactpersonType is niet geldig."; }
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
        #region 'Inlade data'
        public static ObservableCollection<ContactpersonType> GetContactTypes()
        {
            try
            {
                ObservableCollection<ContactpersonType> types = new ObservableCollection<ContactpersonType>();
                string sql = "SELECT * from Jobroles";
                DbDataReader reader = Database.GetData(sql);
                while (reader.Read())
                {
                    types.Add(MakeJobrole(reader));
                }
                ObservableCollection<ContactpersonType> typesSort = new ObservableCollection<ContactpersonType>(from i in types orderby i.Name select i);
                reader.Close();
                return typesSort;
            }
            catch { return new ObservableCollection<ContactpersonType>(); }

        }
        private static ContactpersonType MakeJobrole(DbDataReader reader)
        {
            ContactpersonType cp = new ContactpersonType();
            cp.ID = reader["Id"].ToString();
            cp.Name = reader["JobroleName"].ToString();
            return cp;
        }
        #endregion
        #region 'Save Data'
        public void SaveJobrole()
        {
            int AffectedRow = 0;
            if (this.ID == null || this.ID == "")
            {
                string sql = "Insert INTO Jobroles (JobroleName) VALUES(@JobroleName)";
                DbParameter jobname = Database.AddParameter("@JobroleName", this.Name);
                AffectedRow = Database.ModifyData(sql, jobname);
            }
            else
            {
                string sql = "UPDATE Jobroles SET JobroleName = @naam WHERE Id = @id";
                DbParameter jobname = Database.AddParameter("@naam", this.Name);
                DbParameter id = Database.AddParameter("@id", this.ID);
                AffectedRow = Database.ModifyData(sql, jobname, id);
            }

        }
        #endregion
        #region 'Delete Data'
        public void DeleteJobrole()
        {
            int AffectedRow = 0;
            string sql = "DELETE FROM Jobroles WHERE Id = @id";
            DbParameter id = Database.AddParameter("@id", this.ID);
            AffectedRow = Database.ModifyData(sql, id);
        }
        #endregion

    }
}