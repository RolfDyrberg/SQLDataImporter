/*
 * 
 * SQLServerDataImporter takes an ImportConfiguration and ImportStatements
 * and imports these to the database describes in the ConnectionSetup of the ImportConfiguration.
 * The result of each import is written to an ImportResult.
 * 
 */


using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseConnector;
using SQLDataImporter.StatementCreator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DataImporter
{

    public abstract class DataImporter
    {

        protected ImportConfiguration config;

        public DataImporter(ImportConfiguration config)
        {
            this.config = config;
        }


        public abstract ImportResult[] ImportData(ImportStatement[] statements);

        public abstract ImportResult ImportData(ImportStatement statement);

    }

    public class SQLServerDataImporter : DataImporter
    {

        public SQLServerDataImporter(ImportConfiguration config)
            : base(config) { }

        override public ImportResult[] ImportData(ImportStatement[] statements)
        {
            List<ImportResult> importResults = new List<ImportResult>();

            string connectionString = ConnectionStringMaker.SQLServerConnectionString(config.ConnectionSetup);

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                foreach (ImportStatement s in statements)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = s.SqlStatement;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = sqlConnection;

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        importResults.Add(new SuccesfulImport(s, rowsAffected));
                    }
                    catch (SqlException e)
                    {
                        importResults.Add(new UnsuccesfulImport(s, e.Message));
                    }

                }
            }

            return importResults.ToArray();
        }

        public override ImportResult ImportData(ImportStatement statement)
        {
            string connectionString = ConnectionStringMaker.SQLServerConnectionString(config.ConnectionSetup);

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = statement.SqlStatement;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection;

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return new SuccesfulImport(statement, rowsAffected);
                }
                catch (SqlException e)
                {
                    return new UnsuccesfulImport(statement, e.Message);
                }
            }
        }
        
    }
}
