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
    class Ticket: IDataErrorInfo
    {
        #region 'Fields en constructor'
        private String _id;

        public String ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private String _ticketholder;
        [Required(ErrorMessage="Er moet een eigenaar zijn.")]
        [StringLength(45,MinimumLength=2,ErrorMessage="De lengte moet tussen 2 en 45 karakters zijn.")]
        public String Ticketholder
        {
            get { return _ticketholder; }
            set { _ticketholder = value; }
        }
        private String _ticketholderEmail;
        [EmailAddress(ErrorMessage="Zorg voor een geldig emailadres.")]
        public String TicketholderEmail
        {
            get { return _ticketholderEmail; }
            set { _ticketholderEmail = value; }
        }
        private TicketType _ticketType;
        [Required(ErrorMessage="Selecteer een ticket.")]
        public TicketType TicketType
        {
            get { return _ticketType; }
            set { _ticketType = value; }
        }
        private String _HolderLast;

        public String HolderLast
        {
            get { return _HolderLast; }
            set { _HolderLast = value; }
        }
        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),null, true);
        }
        public override string ToString()
        {
            return Ticketholder + " " + HolderLast + ": " + TicketType.Name;
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
        #region 'Inhalen data'
        public static ObservableCollection<Ticket> GetTickets()
        {
            ObservableCollection<Ticket> orders = new ObservableCollection<Ticket>();
            string sql = "SELECT * FROM OrderedTickets";
            DbDataReader reader = Database.GetData(sql);
            while (reader.Read())
            {
                orders.Add(MaakTicket(reader));
            }
            ObservableCollection<Ticket> ordersSort = new ObservableCollection<Ticket>(from i in orders orderby i.Ticketholder select i);
            reader.Close();
            return ordersSort;
        }

        private static Ticket MaakTicket(DbDataReader reader)
        {
            Ticket t = new Ticket();
            t.ID = reader["Id"].ToString();
            t.Ticketholder = reader["Holder"].ToString();
            t.TicketholderEmail = reader["Email"].ToString();
            t.TicketType = GetTicketType(reader["TicketType"].ToString());
            t.HolderLast = reader["HolderLast"].ToString();
            return t;
        }
        private static Model.TicketType GetTicketType(string p)
        {
            ObservableCollection<TicketType> tt = TicketType.GetTicketTypes();
            foreach (TicketType t in tt)
            {
                if (t.ID.Equals(p)) {return t; }
            }
            return null;
        }
        #endregion
        #region 'Save data'
        public void SaveTicket()
        {
            int iAffectedRows = 0;
            ObservableCollection<Ticket> tickets = Ticket.GetTickets();
            DbParameter id = Database.AddParameter("@ID", this.ID);
            DbParameter HF = Database.AddParameter("@HF", this.Ticketholder);
            DbParameter HL = Database.AddParameter("@HL", this.HolderLast);
            DbParameter Email = Database.AddParameter("@Email", this.TicketholderEmail);
            DbParameter Type = Database.AddParameter("@Type", this.TicketType.ID);
            if (CheckIfAlreadyExist(tickets, this.ID))
            {
                string sql = "UPDATE OrderedTickets SET Holder = @HF, HolderLast = @HL, Email = @Email, TicketType = @Type WHERE Id=@ID";
                iAffectedRows += Database.ModifyData(sql, HF, HL, Email, Type, id);
            }
            else 
            {
                string sql = "INSERT INTO OrderedTickets (Holder, HolderLast, Email, TicketType) VALUES (@HF, @HL,@Email, @Type)";
                iAffectedRows += Database.ModifyData(sql, HF, HL, Email, Type);
            }
            Console.WriteLine(iAffectedRows);
        }

        private bool CheckIfAlreadyExist(ObservableCollection<Ticket> observableCollection, string p)
        {
            foreach (Ticket t in observableCollection)
            {
                if (t.ID.Equals(p))
                { return true; }
            }
            return false;
        }
        #endregion
        #region 'delete data'
        public void DeleteTicket()
        {
            int iAffectedRows = 0;
            string sql = "DELETE FROM OrderedTickets WHERE Id = @ID";
            DbParameter id = Database.AddParameter("@ID", this.ID);
            iAffectedRows += Database.ModifyData(sql, id);
        
        }
        #endregion
    }
}
