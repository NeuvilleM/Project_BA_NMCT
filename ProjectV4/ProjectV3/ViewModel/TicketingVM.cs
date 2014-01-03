using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using ProjectV3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ProjectV3.ViewModel
{
    class TicketingVM : ObservableObject, IPage
    {
        #region 'Fields en constructor'
        public TicketingVM()
        {
            Types = TicketType.GetTicketTypes();
            OrderedTickets = Ticket.GetTickets();
            SelectedType = new TicketType() {Name = "Enter Type?", AvailableTickets = 0, Price= 0 };
            SelectedTicket = new Ticket() { HolderLast = "Last Name?", Ticketholder = "First Name?", TicketholderEmail = "Email" };
        }
        public string Name
        {
            get { return "Ticketing"; }
        }
        private ObservableCollection<Ticket> _orderedTickets;

        public ObservableCollection<Ticket> OrderedTickets
        {
            get { return _orderedTickets; }
            set { _orderedTickets = value;
            OnPropertyChanged("OrderedTickets");
            }
        }

        private ObservableCollection<TicketType> _types;

        public ObservableCollection<TicketType> Types
        {
            get { return _types; }
            set { _types = value;
            OnPropertyChanged("Types");
            }
        }
        private TicketType _selectedType;

        public TicketType SelectedType
        {
            get { return _selectedType; }
            set { _selectedType = value;
            OnPropertyChanged("SelectedType");
            }
        }
        private Ticket _selectedTicket;

        public Ticket SelectedTicket
        {
            get { return _selectedTicket; }
            set { _selectedTicket = value;
            OnPropertyChanged("SelectedTicket");
            }
        }
        
        #endregion
        #region 'Commands For Type'
        #region 'save-command'
        private ICommand _SaveType;

        public ICommand SaveType
        {
            get { return new RelayCommand(SaveExecute, CanExecuteSaveTypes); }
            private set { _SaveType = value; }
        }
        private bool CanExecuteSaveTypes()
        {
            if (SelectedType == null)
            {
                NewExecuteType();
                return false;
            }
            if (SelectedType.Name == "Enter Type?")
                return false;
            if (SelectedType.IsValid()) return true;
            else return false;
        }

        public void SaveExecute()
        {
            SelectedType.SaveType();
            NewExecuteType();         
        }
        #endregion
        #region 'new-command'
        private ICommand _NewType;

        public ICommand NewType
        {
            get { return new RelayCommand(NewExecuteType, CanExecuteNewType); }
            private set { _NewType = value; }
        }
        private bool CanExecuteNewType()
        {
            return true;
        }

        public void NewExecuteType()
        {
            SelectedType = new TicketType() { Name = "Enter Type?", AvailableTickets = 0, Price = 0 };
            Types = TicketType.GetTicketTypes();
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteType;

        public ICommand DeleteType
        {
            get { return new RelayCommand(DeleteExecuteType, CanExecuteDeleteType); }
            private set { _DeleteType = value; }
        }
        private bool CanExecuteDeleteType()
        {
            if (SelectedType.ID == null) { return false; }
            if (SelectedType.ID == "") { return false; }
            return true;
        }
        public void DeleteExecuteType()
        {
            SelectedType.DeleteType();
            SelectedType = new TicketType() { Name = "Enter Type?", AvailableTickets = 0, Price = 0 };
            NewExecuteType();
            
        }
        #endregion
        #endregion
        #region 'Commands For Ticket'
        #region 'save-command'
        private ICommand _SaveTicket;

        public ICommand SaveTicket
        {
            get { return new RelayCommand(SaveExecuteTicket, CanExecuteSaveTicket); }
            private set { _SaveTicket = value; }
        }
        private bool CanExecuteSaveTicket()
        {
            if (SelectedTicket == null)
            {
                NewExecuteTicket();
                return false;
            }
            if (SelectedTicket.Ticketholder == "First Name?")
                return false;
            if (SelectedTicket.IsValid()) return true;
            else return false;
        }

        public void SaveExecuteTicket()
        {
            SelectedTicket.SaveTicket();
            NewExecuteTicket();
        }
        #endregion
        #region 'new-command'
        private ICommand _NewTicket;

        public ICommand NewTicket
        {
            get { return new RelayCommand(NewExecuteTicket, CanExecuteNewTicket); }
            private set { _NewTicket = value; }
        }
        private bool CanExecuteNewTicket()
        {
            return true;
        }

        public void NewExecuteTicket()
        {
            SelectedTicket = new Ticket() { HolderLast = "Last Name?", Ticketholder = "First Name?", TicketholderEmail = "Email" };
            OrderedTickets = Ticket.GetTickets();
        }
        #endregion
        #region 'delete-command'
        private ICommand _DeleteTicket;

        public ICommand DeleteTicket
        {
            get { return new RelayCommand(DeleteExecuteTicket, CanExecuteDeleteTicket); }
            private set { _DeleteTicket = value; }
        }
        private bool CanExecuteDeleteTicket()
        {
            if (SelectedTicket.ID == null) { return false; }
            if (SelectedTicket.ID == "") { return false; }
            return true;
        }
        public void DeleteExecuteTicket()
        {
            SelectedTicket.DeleteTicket();
            NewExecuteTicket();

        }
        #endregion
        #endregion
        #region 'Commands for ticketexport'
        public ICommand SaveFileCommand { get { return new RelayCommand(SaveDataToFile, CanExecuteSaveFileCommand); } }

        private bool CanExecuteSaveFileCommand()
        {
            if (SelectedTicket.ID != null && SelectedTicket.ID != "") return true;
            return false;
        }
        public ICommand SaveFilesCommand { get { return new RelayCommand(SaveDataToFiles); } }
        public void SaveDataToFiles()
        {
            Ticket.SaveDataToFiles(OrderedTickets);
            //try
            //{
            //    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            //    ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //    ofd.Filter = "word-template|*.docx";
            //    if (ofd.ShowDialog() == true)
            //    {
            //        FolderBrowserDialog fbd = new FolderBrowserDialog();
            //        DialogResult sfd = fbd.ShowDialog();

            //        if (sfd == DialogResult.OK)
            //        {
            //            string targetpath = fbd.SelectedPath;
            //            foreach (Ticket t in OrderedTickets)
            //            {
            //                StartFileWegschrijven(ofd.FileName, t, targetpath);
            //            }
            //        }
            //    }
            //}
            //catch  (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

        }
        public void SaveDataToFile()
        {
            Ticket.SaveDataToFile(SelectedTicket);
            //try
            //{
            //    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            //    ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //    ofd.Filter = "word-template|*.docx";
            //    if (ofd.ShowDialog() == true)
            //    {
            //        FolderBrowserDialog fbd = new FolderBrowserDialog();
            //        DialogResult sfd = fbd.ShowDialog();

            //        if (sfd == DialogResult.OK)
            //        {
            //            string targetpath = fbd.SelectedPath;

            //            StartFileWegschrijven(ofd.FileName, SelectedTicket, targetpath);

            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

        }
        public void StartFileWegschrijven(string sBestandsnaam, Ticket ticket, string targetpath)
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
                MessageBox.Show("Zorg ervoor dat alle bookmarks in het document aanwezig zijn: \n FestivalNaam, Aantal, Email, Voornaam, Naam, Ticket, Barcode \n error: "+ex.Message);
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
        }
        #endregion


    }
