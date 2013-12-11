using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectV3.Model
{
    class Festival: IDataErrorInfo
    {
        private String _name;
        [Required(ErrorMessage="De naam voor het festival is verplicht.")]
        [StringLength(50,MinimumLength =2, ErrorMessage="De naam moet tussen de 2 en 45 karakters hebben.")]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        public DateTime _startDate;
        [Required(ErrorMessage="De startdatum is verplicht.")]
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }
        private DateTime _endDate;
        [Required(ErrorMessage="De einddatum is verplicht.")]
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
        private String _imagelink;
        
        public String ImageLink
        {
            get { return _imagelink; }
            set { _imagelink = value; }
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
        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),
            null, true);
        }
        
        public static Festival GetFestival()
        {
            Festival festi = new Festival();
            string sql = "SELECT * from Festival";
            DbDataReader reader = Database.GetData(sql);
            reader.Read();
            festi.EndDate = Convert.ToDateTime(reader["End"].ToString());
            festi.ImageLink = reader["Picture"].ToString();
            festi.Name = reader["FestivalNaam"].ToString();
            festi.StartDate = Convert.ToDateTime(reader["Start"].ToString());
            reader.Close();
            return festi;
        }
        public void SaveFestival()
        {
            DbParameter name = Database.AddParameter("@Name", this.Name);
            DbParameter Start = Database.AddParameter("@Start", MakeDateForSQL(this.StartDate));
            DbParameter End = Database.AddParameter("@End", MakeDateForSQL(this.EndDate));
            DbParameter id = Database.AddParameter("@id", 1);
            DbParameter Pic = Database.AddParameter("@Pic", this.ImageLink);
            string sql = "UPDATE festival SET FestivalNaam=@Name, Start=@Start, End=@End, Picture=@Pic WHERE Id=@id";
            int iAffectedRows = Database.ModifyData(sql, name, Start, End, Pic, id);
        }
        private string MakeDateForSQL(DateTime dateTime)
        {
            string date = Convert.ToString(dateTime.Year) + "/";
            date += Convert.ToString(dateTime.Month) + "/";
            date += Convert.ToString(dateTime.Day);
            return date;
        }
    }

}
