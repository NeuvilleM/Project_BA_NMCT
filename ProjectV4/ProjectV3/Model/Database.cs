using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ProjectV3.Model
{
    class Database
    {
        // vooraf: instellingen ophalen uit config-bestand
        private static ConnectionStringSettings ConnectionString
        {
            get
            {
                // met onderstaande lijn haalt hij alle informatie uit het config bestand op dat te maken heeft met de connectionstring met naam "ConnectionString"
                return ConfigurationManager.ConnectionStrings["ConnectionString"];
            }
        }
        // stap 1: connectie opvragen
        private static DbConnection GetConnection()
        {
            try
            {
                DbConnection con = DbProviderFactories.GetFactory(ConnectionString.ProviderName).CreateConnection();
                con.ConnectionString = ConnectionString.ConnectionString;
                con.Open();
                return con;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // stap 2: connectie vrijgeven
        public static void ReleaseConnection(DbConnection con)
        {
            if (con != null)
            {
                con.Close();
                con = null;
            }
        }
        // stap 3: Command opstellen: sql-string en parameters doorgeven
        private static DbCommand BuildCommand(String sql, params DbParameter[] parameters)
        {
            //params laat toe om de methode opteroepen met slechts 1 parameter, nl. de sql-string
            // intern in deze methode gaan we de connectie leggen met de database
            DbCommand command = GetConnection().CreateCommand();
            // command ~> boodschappenlijstje
            command.CommandType = System.Data.CommandType.Text;
            //sql-string!
            command.CommandText = sql;
            // parameters doorgeven, tegen sql-injection
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            return command;
        }
        // stap 3bis: hulpmethode om parameters te maken
        public static DbParameter AddParameter(string naam, object value)
        {
            // deze methode maakt een parameter aan, die dan later kan 
            // doorgegeven worden via de methode BuildCommand.
            // naar de factory specifiek voor mijn provider (params zijn provider afhankelijk)
            DbParameter par = DbProviderFactories.GetFactory(ConnectionString.ProviderName).CreateParameter();
                
            if (value != null)
            {
                par.ParameterName = naam;
                par.Value = value;
            }
            else {
                par.ParameterName = naam;
            }
            return par;
        }
        // stap 4a: Data ophalen (select-statements)
        public static DbDataReader GetData(string sql, params DbParameter[] parameters)
        {
            // zie vorige methode
            DbCommand command = null;
            DbDataReader reader = null;
            try
            {
                command = BuildCommand(sql, parameters);
                // op onderstaande lijn wordt naar database gegaan, er wordt met een 
                // datareader teruggekeerd.
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                //ReleaseConnection(command.Connection);
                return reader;
            }
            catch (Exception ex)
            {
                // afprinten wat verkeerd is.
                Console.WriteLine(ex.Message);
                if (reader != null) reader.Close();
                if (command != null) ReleaseConnection(command.Connection);

                // fout doorgeven aan de aanroeper
                throw ex;
            }
            
        }
        // stap 4b: data wijzigen (insert, delete, update)
        public static int ModifyData(String sql, params DbParameter[] parameters)
        {
            DbCommand command = null;
            try
            {
                command = BuildCommand(sql, parameters);
                int aantalRijenGewijzigd = command.ExecuteNonQuery();
                // aantal verwijderen/toegevoegd/aangepaste rijden wordt teruggegeven
                // zo heeft de gebruiker controle of het gelukt is of niet.
                ReleaseConnection(command.Connection);
                return aantalRijenGewijzigd;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (command != null) ReleaseConnection(command.Connection);
                // fout doorgeven aan de aanroeper
                return 0;
            }
        }

        // EXTRA: werken met transacties
        // vooraf: Transactie aanmaken (waarin alle commando's ofwel lukken, ofwel niet lukken)
        public static DbTransaction BeginTransaction()
        {
            DbConnection con = null;
            try
            {
                con = GetConnection();
                // transactie aanmaken en teruggeven
                return con.BeginTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (con != null) ReleaseConnection(con);
                throw ex;
            }
        }
        // Stap 3 extra: command in functie van transactie
        private static DbCommand BuildCommand(DbTransaction trans, string sql, params DbParameter[] parameters)
        {
            DbCommand command = trans.Connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sql;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            return command;
        }
        // stap 4 extra a: data ophalen binnen in een transactie
        public static DbDataReader GetData(DbTransaction trans, string sql, params DbParameter[] parameters)
        {
            // zie vorige methode
            DbCommand command = null;
            DbDataReader reader = null;
            try
            {
                command = BuildCommand(trans, sql, parameters);
                // op onderstaande lijn wordt naar database gegaan, er wordt met een 
                // datareader teruggekeerd.
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
               // ReleaseConnection(command.Connection);
                return reader;
            }
            catch (Exception ex)
            {
                // afprinten wat verkeerd is.
                Console.WriteLine(ex.Message);
                if (reader != null) reader.Close();
                if (command != null) ReleaseConnection(command.Connection);
                // fout doorgeven aan de aanroeper
                throw ex;
            }
        }
        // stap 4 extra b: data bewereken binnen in een transactie
        public static int ModifyData(DbTransaction trans, String sql, params DbParameter[] parameters)
        {
            DbCommand command = null;
            try
            {
                command = BuildCommand(trans, sql, parameters);
                int aantalRijenGewijzigd = command.ExecuteNonQuery();
                // aantal verwijderen/toegevoegd/aangepaste rijden wordt teruggegeven
                // zo heeft de gebruiker controle of het gelukt is of niet.
                ReleaseConnection(command.Connection);
                return aantalRijenGewijzigd;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (command != null) ReleaseConnection(command.Connection);
                // fout doorgeven aan de aanroeper
                throw ex;
            }
        }
    }
}
