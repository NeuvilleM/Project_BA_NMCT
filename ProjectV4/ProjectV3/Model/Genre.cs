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
    class Genre: IDataErrorInfo
    {
        #region 'fields'
        private String _id;

        public String ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private String _name;
        [Required(ErrorMessage="De naam van het genre is verplicht.")]
        [StringLength(45,MinimumLength=2,ErrorMessage="De lengte moet tussen de 2 en de 45 karakters liggen.")]
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
        #region 'Inladen data'
        public static ObservableCollection<Genre> GetGenres()
        {
            try
            {
                ObservableCollection<Genre> genres = new ObservableCollection<Genre>();
                string sql = "SELECT * FROM Genres";
                DbDataReader reader = Database.GetData(sql);
                while (reader.Read())
                {
                    genres.Add(MaakGenre(reader));
                }
                ObservableCollection<Genre> genresSort = new ObservableCollection<Genre>(from i in genres orderby i.Name select i);
                reader.Close();
                return genresSort;
            }
            catch
            {
                return new ObservableCollection<Genre>();
            }
        }
        private static Genre MaakGenre(DbDataReader reader)
        {
            Genre g = new Genre();
            g.ID = reader["Id"].ToString();
            g.Name = reader["GenreNaam"].ToString();
            return g;
        }
        #endregion
        #region 'Save data'
        public void SaveGenre()
        {
            int AffectedRow = 0;
            if (this.ID == null || this.ID == "")
            {
                string sql = "Insert INTO Genres (GenreNaam) VALUES(@GenreNaam)";
                DbParameter genren = Database.AddParameter("@GenreNaam", this.Name);
                AffectedRow = Database.ModifyData(sql, genren);
            }
            else
            {
                string sql = "UPDATE Genres SET GenreNaam = @naam WHERE Id = @id";
                DbParameter genren = Database.AddParameter("@naam", this.Name);
                DbParameter id = Database.AddParameter("@id", this.ID);
                AffectedRow = Database.ModifyData(sql, genren, id);
            }
        
        }
        #endregion
        #region 'Delete genre'
        public void DeleteGenre() 
        {
            int AffectedRow = 0;
            string sql = "DELETE FROM Genres WHERE Id = @id";
            DbParameter id = Database.AddParameter("@id", this.ID);
            AffectedRow = Database.ModifyData(sql, id);
        }
        #endregion
    }
}
