using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectV3.Model
{
    class TicketType: IDataErrorInfo
    {
        #region 'Fields'
        private String _id;

        public String ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private String _name;
        [Required(ErrorMessage="Je moet een naam opgeven")]
        [StringLength(45, MinimumLength=2, ErrorMessage="De naam moet tussen de 2 en 45 karakters bevatten.")]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private Double _price;
        [Required(ErrorMessage="Er moet een prijs meegegeven worden")]
        [Range(0, Double.MaxValue,ErrorMessage="Geef een positieve waarde op.")]
        public Double Price
        {
            get { return _price; }
            set { _price = value; }
        }
        private int _availableTickets;
        [Required(ErrorMessage="Je moet het aantal beschikbare ticketen meegeven.")]
        [Range(0, int.MaxValue,ErrorMessage="Geef een positieve waarde op.")]
        public int AvailableTickets
        {
            get { return _availableTickets; }
            set { _availableTickets = value; }
        }
        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),
            null, true);
        }
        public override string ToString()
        {
            return Name + ": " + Price;
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
        #region 'Ophalen data'
        public static ObservableCollection<TicketType> GetTicketTypes()
        {
            ObservableCollection<TicketType> types = new ObservableCollection<TicketType>();
            string sql = "SELECT * from TicketType";
            DbDataReader reader = Database.GetData(sql);
            while (reader.Read())
            {
                types.Add(MaakTicketType(reader));
            }
            ObservableCollection<TicketType> typesSort = new ObservableCollection<TicketType>(from i in types orderby i.Name select i);
            reader.Close();
            return types;
        }
        private static TicketType MaakTicketType(DbDataReader reader)
        {
            TicketType tp = new TicketType();
            tp.Price = Convert.ToDouble(reader["price"].ToString());
            tp.Name = reader["TicketName"].ToString();
            tp.ID = reader["Id"].ToString();
            tp.AvailableTickets = Convert.ToInt32(reader["Available"].ToString());
            return tp;
        }
        #endregion
        #region 'Save data'
        public void SaveType()
        {
            if(this != null)
            {
                int iAffectedRows = 0;
                ObservableCollection<TicketType> types = TicketType.GetTicketTypes();
                DbParameter Id = Database.AddParameter("@ID", this.ID);
                DbParameter Types = Database.AddParameter("@Type", this.Name);
                DbParameter Price = Database.AddParameter("@Price", this.Price);
                DbParameter Available = Database.AddParameter("@Ava", this.AvailableTickets);
            if (CheckIfAlreadyExist(types, this.ID))
            {
             string sql = "UPDATE TicketType SET TicketName = @Type, price = @Price, Available = @Ava WHERE Id = @ID";
             iAffectedRows += Database.ModifyData(sql, Types, Price, Available, Id);
            }
            else 
            {
                string sql = "INSERT INTO TicketType (TicketName, price, Available) VALUES (@Type, @Price, @Ava)";
                iAffectedRows += Database.ModifyData(sql, Types, Price, Available);
            }
            Console.WriteLine(iAffectedRows);
            }
        
        }

        private bool CheckIfAlreadyExist(ObservableCollection<TicketType> observableCollection, string p)
        {
            foreach (TicketType tt in observableCollection)
            {
                if (tt.ID.Equals(p)) { return true; }            
            }
            return false;
        }
        #endregion
        public void DeleteType()
        {
            int iAffectedRows = 0;
            DbParameter id = Database.AddParameter("@ID", this.ID);
            string sql = "DELETE FROM TicketType WHERE Id = @ID";
            iAffectedRows += Database.ModifyData(sql, id);        
        }
    }
}
