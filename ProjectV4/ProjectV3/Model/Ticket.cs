using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

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
        [Required]
        public int Number { get; set; }
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
            get { return "Object Stage is niet geldig."; }
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
            try
            {
                ObservableCollection<Ticket> orders = new ObservableCollection<Ticket>();
                string sql = "SELECT * FROM orderedtickets";
                DbDataReader reader = Database.GetData(sql);
                while (reader.Read())
                {
                    orders.Add(MaakTicket(reader));
                }
                ObservableCollection<Ticket> ordersSort = new ObservableCollection<Ticket>(from i in orders orderby i.Ticketholder select i);
                reader.Close();
                return ordersSort;
            }
            catch
            { return new ObservableCollection<Ticket>(); }
        }

        private static Ticket MaakTicket(DbDataReader reader)
        {
            Ticket t = new Ticket();
            t.ID = reader["Id"].ToString();
            t.Ticketholder = reader["Holder"].ToString();
            t.TicketholderEmail = reader["Email"].ToString();
            t.TicketType = GetTicketType(reader["TicketType"].ToString());
            t.HolderLast = reader["HolderLast"].ToString();
            int i = 0;
            Int32.TryParse(reader["Number"].ToString(), out i);
            t.Number = i;
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
            DbParameter N = Database.AddParameter("@N", this.Number);
            if (CheckIfAlreadyExist(tickets, this.ID))
            {
                string sql = "UPDATE OrderedTickets SET Holder = @HF, HolderLast = @HL, Email = @Email, TicketType = @Type, Number = @N WHERE Id=@ID";
                iAffectedRows += Database.ModifyData(sql, HF, HL, Email, Type,N, id);
            }
            else 
            {
                string sql = "INSERT INTO OrderedTickets (Holder, HolderLast, Email, TicketType,Number) VALUES (@HF, @HL,@Email, @Type,@N)";
                iAffectedRows += Database.ModifyData(sql, HF, HL, Email, Type,N);
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
        #region 'Tickets Printen'
        public static void SaveDataToFiles(ObservableCollection<Ticket> OrderedTickets)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                ofd.Filter = "word-template|*.docx";
                if (ofd.ShowDialog() == true)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    DialogResult sfd = fbd.ShowDialog();

                    if (sfd == DialogResult.OK)
                    {
                        string targetpath = fbd.SelectedPath;
                        foreach (Ticket t in OrderedTickets)
                        {
                            StartFileWegschrijven(ofd.FileName, t, targetpath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public static void SaveDataToFile(Ticket SelectedTicket)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                ofd.Filter = "word-template|*.docx";
                if (ofd.ShowDialog() == true)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    DialogResult sfd = fbd.ShowDialog();

                    if (sfd == DialogResult.OK)
                    {
                        string targetpath = fbd.SelectedPath;

                        StartFileWegschrijven(ofd.FileName, SelectedTicket, targetpath);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public static void StartFileWegschrijven(string sBestandsnaam, Ticket ticket, string targetpath)
        {
            //int iTeller = 0;


            if (!System.IO.Directory.Exists(targetpath)) { System.IO.Directory.CreateDirectory(targetpath); }

            string filenaam = ticket.Ticketholder + "_" + ticket.HolderLast + "_" + ticket.ID + ".docx";
            string destinationfile = System.IO.Path.Combine(targetpath, filenaam);
            File.Copy(sBestandsnaam, destinationfile, true);
            WordprocessingDocument newdoc = WordprocessingDocument.Open(destinationfile, true);
            try
            {
                IDictionary<String, BookmarkStart> bookmarks = new Dictionary<String, BookmarkStart>();
                foreach (BookmarkStart bms in newdoc.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                {
                    bookmarks[bms.Name] = bms;
                }
                Festival f = new Festival();
                f = Festival.GetFestival();
                if (f.Name != null)
                {
                    bookmarks["FestivalNaam"].Parent.InsertAfter<Run>(new Run(new Text(f.Name)), bookmarks["FestivalNaam"]);
                }
                bookmarks["Aantal"].Parent.InsertAfter<Run>(new Run(new Text(ticket.Number.ToString())), bookmarks["Aantal"]);
                bookmarks["Email"].Parent.InsertAfter<Run>(new Run(new Text(ticket.TicketholderEmail)), bookmarks["Email"]);
                bookmarks["Voornaam"].Parent.InsertAfter<Run>(new Run(new Text(ticket.Ticketholder)), bookmarks["Voornaam"]);
                bookmarks["Naam"].Parent.InsertAfter<Run>(new Run(new Text(ticket.HolderLast)), bookmarks["Naam"]);
                bookmarks["Ticket"].Parent.InsertAfter<Run>(new Run(new Text(ticket.TicketType.Name)), bookmarks["Ticket"]);

                DocumentFormat.OpenXml.Wordprocessing.Table t = new DocumentFormat.OpenXml.Wordprocessing.Table();
                for (int tic = 1; tic <= ticket.Number; tic++)
                {
                    //bookmarks["Barcode"].Parent.InsertAfter<Run>(new Run(new Text(" barcode "+tic.ToString()+" : ")), bookmarks["Barcode"]);

                    string val = "T" + ticket.ID + "y" + ticket.TicketType.ID + "c" + tic.ToString(); ;

                    Run run = new Run(new Text(val));
                    RunProperties prop = new RunProperties();
                    RunFonts font = new RunFonts() { Ascii = "Free 3 of 9 Extended", HighAnsi = "Free 3 of 9 Extended" };

                    FontSize size = new FontSize() { Val = "72" };
                    prop.Append(font);
                    prop.Append(size);
                    run.PrependChild<RunProperties>(prop);
                    TableRow tr1 = new TableRow();
                    TableCell tc1 = new TableCell(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("barcode " + tic.ToString() + ": "))));
                    TableCell tc2 = new TableCell(new Paragraph(run));
                    tr1.Append(tc1, tc2);
                    t.AppendChild(tr1);

                }
                bookmarks["Barcode"].Parent.InsertAfter<DocumentFormat.OpenXml.Wordprocessing.Run>(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Table(t)), bookmarks["Barcode"]);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Zorg ervoor dat alle bookmarks in het document aanwezig zijn: \n FestivalNaam, Aantal, Email, Voornaam, Naam, Ticket, Barcode \n error: " + ex.Message);
                throw new System.InvalidOperationException("Not all bookmarks where present.");
            }
            finally
            {
                if (newdoc != null)
                {
                    newdoc.Close();
                }
            }
        }          
        #endregion
    }
}
