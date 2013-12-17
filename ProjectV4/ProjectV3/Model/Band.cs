using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ProjectV3.ViewModel;
using System.Data.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectV3.Model
{
    class Band:  IComparable, IDataErrorInfo
    {
        #region fields
        private String _id;

        public String ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private String _name;
        [Required (ErrorMessage= "De naam van de band is verplict")]
        [StringLength(50, MinimumLength=2, ErrorMessage="De naam moet tussen 2 en 50 karakters lang zijn.")]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private String _picture;
        
        [Url (ErrorMessage="Voer een correcte url, of niets in.")]
        public String Picture
        {
            get { return _picture; }
            set { _picture = value; }
        }
        private String _description;
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private String _twitter;
        [StringLength(15, ErrorMessage="De twitteraccount mag max. 15 tekens bevatten.")]
        public String Twitter
        {
            get { return _twitter; }
            set { _twitter = value; }
        }

        private String _facebook;
        [StringLength(45, ErrorMessage="De lengte mag max. 45 karakters hebben, start na facebook.com/")]
        public String Facebook
        {
            get { return _facebook; }
            set { _facebook = value; }
        }
        private ObservableCollection<Genre> _genres;
        public ObservableCollection<Genre> Genres
        {
            get { return _genres; }
            set { _genres = value; }
        }
        #endregion
        #region 'Ophalen data van database'
        public static ObservableCollection<Band> GetArtists()
        {
            try
            {
                ObservableCollection<Genre> g = Genre.GetGenres();
                ObservableCollection<Band> artiesten = new ObservableCollection<Band>();
                string sql = "SELECT Artist.Id, Artist.Picture, Artist.Naam, Artist.Description, Artist.Twitter, Artist.Facebook, Artist_Genre.ArtistID, Artist_Genre.GenreID";
                sql += " FROM Artist INNER JOIN Artist_Genre ON Artist.Id = Artist_Genre.ArtistID";
                DbDataReader reader = Database.GetData(sql);
                // overloop elkereader["Id"].ToString()) rij en maak er een object van
                while (reader.Read())
                {
                    if (CheckIfAlreadyExist(artiesten, reader["Id"].ToString()))
                    {
                        artiesten = UpdateArtiestGenre(artiesten, reader, g);
                    }
                    else
                    {
                        artiesten.Add(MaakArtist(reader, g));
                        artiesten = UpdateArtiestGenre(artiesten, reader, g);
                    }
                }
                artiesten.OrderBy(x => x.Name);
                ObservableCollection<Band> artiestenSort = new ObservableCollection<Band>(from i in artiesten orderby i.Name select i);
                reader.Close();
                return artiestenSort;
            }
            catch 
            {
                return new ObservableCollection<Band>();
            }
        }
        private static ObservableCollection<Band> UpdateArtiestGenre(ObservableCollection<Band> artiesten, DbDataReader reader, ObservableCollection<Genre> genres)
        {
            try
            {
                Band teUpdaten = ZoekBand(artiesten, reader["Id"].ToString());
                int i = artiesten.IndexOf(teUpdaten);
                Genre g = ZoekGenre(genres, reader["GenreID"].ToString());
                if (g != null)
                {
                    teUpdaten.Genres.Add(ZoekGenre(genres, reader["GenreID"].ToString()));
                }
                artiesten[i] = teUpdaten;

                return artiesten;
            }
            catch {
                return artiesten;
            }
        }
        private static Genre ZoekGenre(ObservableCollection<Genre> genres, string p)
        {
            foreach (Genre g in genres)
            {
                if (g.ID.Equals(p))
                    return g;
            }
            return null;
        }
        private static Band ZoekBand(ObservableCollection<Band> artiesten, string p)
        {
            foreach (Band b in artiesten)
            {
                if (b.ID.Equals(p))
                    return b;
            }
            return new Band();
        }
        public static Band MaakArtist(DbDataReader reader, ObservableCollection<Genre> g)
        {
            Band nieuw = new Band();
            nieuw.Description = reader["Description"].ToString();
            nieuw.Facebook = reader["Facebook"].ToString();
            //string[] arrstukjes = reader["Genres"].ToString().Split(',');
            //nieuw.Genres = MaakGenresAan(reader["Id"].ToString(), g);
            nieuw.Genres = new ObservableCollection<Genre>();
            nieuw.ID = reader["Id"].ToString();
            nieuw.Name = reader["Naam"].ToString();
            nieuw.Picture = reader["Picture"].ToString();
            nieuw.Twitter = reader["Twitter"].ToString();
            return nieuw;
        }
        #endregion
        #region 'Save acties'
        public void SaveArtist()
        {
            try
            {
                int iAffectedRows = 0;
                ObservableCollection<Band> artiesten = GetArtists();
                DbParameter Pic = Database.AddParameter("@Picture", this.Picture);
                DbParameter Name = Database.AddParameter("@Naam", this.Name);
                DbParameter Desc = Database.AddParameter("@Description", this.Description);
                DbParameter Twit = Database.AddParameter("@Twitter", this.Twitter);
                DbParameter Face = Database.AddParameter("@Facebook", this.Facebook);
                DbParameter id = Database.AddParameter("@ID", this.ID);
                if (CheckIfAlreadyExist(artiesten, this.ID))
                {
                    string sql = "UPDATE Artist SET Picture = @Picture, Naam = @Naam, Description = @Description, Twitter = @Twitter, Facebook=@Facebook WHERE ID = @ID";
                    iAffectedRows += Database.ModifyData(sql, Pic, Name, Desc, Twit, Face, id);
                    UpdateGenresInDatabase(this.ID, this.Genres);
                }
                else
                {

                    string sql = "INSERT INTO Artist (Picture, Naam, Description, Twitter, Facebook)VALUES (@Picture, @Naam, @Description, @Twitter, @Facebook)";
                    iAffectedRows += Database.ModifyData(sql, Pic, Name, Desc, Twit, Face);
                    InsertGenresInDatabase(this.Name, this.Genres);
                }
                Console.WriteLine(iAffectedRows);
            }
            catch { Console.WriteLine("A problem occured while saving data"); }
            
        }
        private void InsertGenresInDatabase(string name, ObservableCollection<Genre> observableCollection)
        {
            
           // zoek band id in database
            string sql = "SELECT * from Artist WHERE Naam = @name";
            DbParameter Name = Database.AddParameter("@name", name);
            DbDataReader reader = Database.GetData(sql, Name);
            reader.Read();
            // voeg alle niewe genres toe
            AddGenres(reader["Id"].ToString(), observableCollection);
            reader.Close();
        }
        private int AddGenres(string bandID, ObservableCollection<Genre> observableCollection)
        {
            int iAffected = 0;
            if (observableCollection.Count == 0)
            {
                string sql = "INSERT INTO Artist_Genre VALUES(@aid,@gid)";
                DbParameter ArtistId = Database.AddParameter("@aid", bandID);
                DbParameter genreID = Database.AddParameter("@gid", 15);
                iAffected += Database.ModifyData(sql, ArtistId, genreID);
            }
            foreach (Genre g in observableCollection)
            {
                string sql = "INSERT INTO Artist_Genre VALUES(@aid,@gid)";
                DbParameter ArtistId = Database.AddParameter("@aid", bandID);
                DbParameter genreID = Database.AddParameter("@gid", g.ID);
                iAffected += Database.ModifyData(sql, ArtistId, genreID);
                //genreID.ResetDbType();
            }
            return iAffected;
        }
        private void UpdateGenresInDatabase(string id,ObservableCollection<Genre> observableCollection )
        {
            // verwijder genres in database
            string sql = "DELETE FROM Artist_Genre WHERE ArtistID = @AId";
            DbParameter ID = Database.AddParameter("AId", id);
            int iaffectedrows = Database.ModifyData(sql, ID);
            // voeg alle nieuwe genres toe
            AddGenres(id, observableCollection);
        }
        private string ZoekGenres(ObservableCollection<Genre> observableCollection)
        {
            string genres = "";
            foreach (Genre g in observableCollection)
            {
                genres += g.ID;
                genres += ',';
            }
            return genres;
        }
        private static bool CheckIfAlreadyExist(ObservableCollection<Band> observableCollection, string p)
        {
            int i = 0;
            if (Int32.TryParse(p, out i))
            {
                foreach (Band b in observableCollection)
                {
                    if (b.ID.Equals(p)) { return true; }
                }
                return false;
            }
            return false;
        }
        #endregion
        #region 'Delete data'
        public void DeleteArtist()
        {
            // controleren of de artiest al in de database zit
            
            // artist wissen uit database
            string sql = "DELETE FROM Artist WHERE Id = @id";
            DbParameter id = Database.AddParameter("@id", this.ID);
            int affectedrows = 0;
            affectedrows += Database.ModifyData(sql, id);

        }
        #endregion
        #region 'Implemented functions'
        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            Band b = obj as Band;
            return this.Name.CompareTo(b.Name);
        }
        public bool IsValid()
        {
        return Validator.TryValidateObject(this, new ValidationContext(this, null, null),null, true);
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
    }
}
