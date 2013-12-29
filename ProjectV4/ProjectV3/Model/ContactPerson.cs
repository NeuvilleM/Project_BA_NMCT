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
    class ContactPerson: IDataErrorInfo
    {
        #region 'Fields'
        private String _id;

        public String ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private String _name;
        [Required(ErrorMessage="De naam is verplicht")]
        [StringLength(45, MinimumLength=2, ErrorMessage="De naam moet tussen 2,45 karakters zijn.")]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private String _company;
        [Required(ErrorMessage="Bedrijf is verplicht")]
        [StringLength(45, MinimumLength = 2, ErrorMessage = "Het bedrijf moet tussen 2,45 karakters zijn.")]
        public String Company
        {
            get { return _company; }
            set { _company = value; }
        }
        private ContactpersonType _jobRole;
        [Required(ErrorMessage="Je moet verplicht een functie opgeven")]
       
        public ContactpersonType JobRole
        {
            get { return _jobRole; }
            set { _jobRole = value; }
        }
        private String _city;

        public String City
        {
            get { return _city; }
            set { _city = value; }
        }
        private String _email;
        [Required(ErrorMessage="Je moet een email meegeven")]
        [EmailAddress(ErrorMessage="Zorg voor een correct emailformaat")]
        [StringLength(45,ErrorMessage="De email moet tussen 45 en 5 tekens zijn.")]
        public String Email
        {
            get { return _email; }
            set { _email = value; }
        }
        private String _phone;

        public String Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }
        private String _cellphone;

        public String Cellphone
        {
            get { return _cellphone; }
            set { _cellphone = value; }
        }
       
#endregion
        #region 'Inladen Data'
        public static ObservableCollection<ContactPerson> GetContacts()
        {
            try
            {
                ObservableCollection<ContactPerson> cps = new ObservableCollection<ContactPerson>();
                string sql = "SELECT * from Contact";
                DbDataReader reader = Database.GetData(sql);
                while (reader.Read())
                {
                    cps.Add(MakeContact(reader));
                }
                ObservableCollection<ContactPerson> cpsSort = new ObservableCollection<ContactPerson>(from i in cps orderby i.Name select i);
                reader.Close();
                return cpsSort;
            }
            catch
            {
                return new ObservableCollection<ContactPerson>();
            }
        }
        private static ContactPerson MakeContact(DbDataReader reader)
        {
            ContactPerson cp = new ContactPerson();
            cp.Cellphone = reader["Cellphone"].ToString();
            cp.City = reader["City"].ToString();
            cp.Company = reader["Company"].ToString();
            cp.Email = reader["Email"].ToString();
            cp.ID = reader["Id"].ToString();
            cp.Name = reader["Naam"].ToString();
            cp.Phone = reader["Phone"].ToString();
            cp.JobRole = GetJobrole(reader["Jobrole"].ToString());
            return cp;
        }
        private static ContactpersonType GetJobrole(string p)
        {
            ObservableCollection<ContactpersonType> cpt = ContactpersonType.GetContactTypes();
            ContactpersonType CPType = new ContactpersonType();
            int i = -1;
            if(Int32.TryParse(p, out i))
                {
                foreach(ContactpersonType c in cpt)
                {
                    if(c.ID.Equals(p)){CPType = c;}
                }
                return CPType;
            }
            else 
            { 
                return null;
            }
        }
        #endregion
        #region 'Save Data'
        public void SaveContact()
        {
            int iAffectedRows = 0;
            ObservableCollection<ContactPerson> contacts = ContactPerson.GetContacts();
            DbParameter name = Database.AddParameter("@Name", this.Name);
            DbParameter Comp = Database.AddParameter("@Comp", this.Company);
            DbParameter jobr = Database.AddParameter("@jobr", GetJobroleID(this.JobRole));
            DbParameter city = Database.AddParameter("@City", this.City);
            DbParameter Email = Database.AddParameter("@Email", this.Email);
            DbParameter Phone = Database.AddParameter("@Phone", this.Phone);
            DbParameter CellP = Database.AddParameter("@CellP", this.Cellphone);
            DbParameter id = Database.AddParameter("@ID", this.ID);
            if (CheckIfAlreadyExist(contacts, this.ID))
            {
                string sql = "UPDATE Contact SET Naam = @Name, Company = @Comp,Jobrole = @jobr, City = @City, Email = @Email, Phone = @Phone, Cellphone = @Cellp WHERE ID = @ID";
                iAffectedRows += Database.ModifyData(sql, name, Comp, jobr, city, Email, Phone, CellP, id);
            }
            else
            {
                string sql = "INSERT INTO Contact (Naam, Company,Jobrole, City,Email, Phone, Cellphone)VALUES (@Name,@Comp,@jobr,@City,@Email,@Phone,@Cellp)";
                iAffectedRows += Database.ModifyData(sql, name, Comp, jobr, city, Email, Phone, CellP);
            }
        }

        private object GetJobroleID(ContactpersonType contactpersonType)
        {
            string ID = "0";
            if (contactpersonType.ID != "0" && contactpersonType.ID != null)
            {
                ID = contactpersonType.ID;
            }
            return ID;
        }

        private bool CheckIfAlreadyExist(ObservableCollection<ContactPerson> observableCollection, string p)
        {
            foreach (ContactPerson cp in observableCollection)
            {
                if (cp.ID.Equals(p)) { return true; }
            }
            return false;
        }
        #endregion
        #region 'Delete Contact'
        public void DeleteContact() 
        {
            string sql = "DELETE FROM Contact WHERE Id = @id";
            DbParameter id = Database.AddParameter("@id", this.ID);
            int affectedRows = Database.ModifyData(sql, id);
            Console.WriteLine("Er zijn " + affectedRows + "contactpersonen verwijderdt.");
        
        }
        #endregion
        #region 'Implemented methodes
        public override string ToString()
        {
            return Name + " " + Company;
        }    
        public string Error
{
    get { return "Object ContactPerson is niet correct."; }
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
            //return true;
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),
            null, true);
        }
#endregion
}
}
