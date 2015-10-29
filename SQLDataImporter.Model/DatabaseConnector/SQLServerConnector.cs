/*
 * 
 * SQLServerConnector connects to a database and reads it into Database object
 * 
 */


using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DatabaseConnector
{
    public class SQLServerConnector : IDatabaseConnector
    {

        ConnectionSetup connectionSetup;

        public SQLServerConnector(ConnectionSetup connectionSetup)
        {
            this.connectionSetup = connectionSetup;
        }

        
        private SqlConnection createSqlConnection()
        {
            return new SqlConnection(ConnectionStringMaker.SQLServerConnectionString(connectionSetup));
        }


        private void setColumns(List<DBTable> tables, Database database)
        {
            string schemaCol = "SCHEMA";
            string tableCol = "TABLE";
            string columnCol = "COLUMN";
            string pkCol = "PRIMARY_KEY";
            string dataTypeCol = "DATA_TYPE";

            string command = "SELECT C.TABLE_SCHEMA AS [{0}], C.TABLE_NAME AS [{1}], C.COLUMN_NAME AS [{2}], CASE WHEN PK.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS [{3}], C.DATA_TYPE AS {4} FROM {5}.INFORMATION_SCHEMA.COLUMNS C LEFT JOIN (SELECT TC.CONSTRAINT_CATALOG, TC.CONSTRAINT_SCHEMA, TC.TABLE_NAME, KCU.COLUMN_NAME FROM {5}.INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC INNER JOIN {5}.INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU ON KCU.CONSTRAINT_NAME = TC.CONSTRAINT_NAME WHERE CONSTRAINT_TYPE = 'PRIMARY KEY') PK ON C.TABLE_CATALOG = PK.CONSTRAINT_CATALOG AND C.TABLE_SCHEMA = PK.CONSTRAINT_SCHEMA AND C.TABLE_NAME = PK.TABLE_NAME AND C.COLUMN_NAME = PK.COLUMN_NAME";

            List<DBColumn> columns = new List<DBColumn>();

            using (SqlConnection sqlConnection = createSqlConnection())
            {

                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = String.Format(command, schemaCol, tableCol, columnCol, pkCol, dataTypeCol, database.Name);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string schema = (string)reader[schemaCol];
                        string tableName = (string)reader[tableCol];
                        string columnName = (string)reader[columnCol];
                        int pk = (int)reader[pkCol];
                        bool isPk = false;
                        if ((int)reader[pkCol] == 1) isPk = true;

                        DBDatatype dataType = stringToDatatype((string)reader[dataTypeCol]);

                        DBTable table = tables.Where<DBTable>(t => t.Name == tableName && t.Schema == schema).FirstOrDefault();
                        if (table != null)
                        {
                            DBColumn column = new DBColumn(table, columnName, isPk, dataType);
                            table.addColumn(column);
                        }
                    }
                }
            }
        }


        private DBDatatype stringToDatatype(string dataType)
        {
            switch (dataType)
            {
                case "int":
                    return DBDatatype.integer;
                case "nvarchar":
                    return DBDatatype.nvarchar;
                case "varchar":
                    return DBDatatype.varchar;
                case "datetime":
                    return DBDatatype.datetime;
                default:
                    return DBDatatype.nvarchar;
            }
        }

        private List<DBTable> getTables(Database database)
        {

            string tablCol = "TABLE";
            string schemaCol = "SCEMA";
            string command = "SELECT TABLE_NAME AS [{0}], TABLE_SCHEMA AS [{1}] FROM {2}.INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME ASC";

            List<DBTable> tables = new List<DBTable>();

            using (SqlConnection sqlConnection = createSqlConnection())
            {

                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = String.Format(command, tablCol, schemaCol, database.Name);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string schema = (string) reader[schemaCol];
                        string tableName = (string) reader[tablCol];
                        tables.Add(new DBTable(schema, tableName));   
                    }
                }
            }

            setColumns(tables, database);

            return tables;
        }

        private void setForeignKeys(Database database) 
        {
            string fkSchemaCol = "FK_SCHEMA";
            string fkTableCol = "FK_TABLE";
            string fkColumnCol = "FK_COLUMN";
            string pkSchemaCol = "PK_SCHEMA";
            string pkTableCol = "PK_TABLE";
            string pkColumnCol = "PK_COLUMN";

            string command = "SELECT FK.TABLE_SCHEMA as [{0}], FK.TABLE_NAME AS [{1}], FK.COLUMN_NAME AS [{2}], PK.TABLE_SCHEMA AS [{3}], PK.TABLE_NAME AS [{4}], PK.COLUMN_NAME AS [{5}] FROM  {6}.INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC INNER JOIN {6}.INFORMATION_SCHEMA.KEY_COLUMN_USAGE FK ON FK.CONSTRAINT_NAME = RC.CONSTRAINT_NAME INNER JOIN {6}.INFORMATION_SCHEMA.KEY_COLUMN_USAGE PK ON PK.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME";


            using (SqlConnection sqlConnection = createSqlConnection())
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = String.Format(command, fkSchemaCol, fkTableCol, fkColumnCol, pkSchemaCol, pkTableCol, pkColumnCol, database.Name);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string fkSchemaName = (string)reader[fkSchemaCol];
                        string fkTableName = (string)reader[fkTableCol];
                        string fkColumnName = (string)reader[fkColumnCol];

                        string pkSchemaName = (string)reader[pkSchemaCol];
                        string pkTableName = (string)reader[pkTableCol];
                        string pkColumnName = (string)reader[pkColumnCol];

                        DBColumn fkColumn = database.GetColumn(fkSchemaName, fkTableName, fkColumnName);
                        DBColumn pkColumn = database.GetColumn(pkSchemaName, pkTableName, pkColumnName);

                        if (fkColumn != null && pkColumn != null)
                        {
                            database.AddForeignKeys(new DBForeignKey(null, pkColumn, null, fkColumn));
                        }
                    }
                }
            }
        }

        public List<string> GetDatabaseNames()
        {
            string dbCol = "DB";
            string command = "SELECT name AS [{0}] FROM sys.databases WHERE HAS_DBACCESS(name) = 1 ORDER BY name ASC";

            List<string> databases = new List<string>();

            using (SqlConnection sqlConnection = createSqlConnection())
            {

                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = String.Format(command, dbCol);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string dbName = String.Format("{0}", reader[dbCol]);
                        databases.Add(dbName);
                    }
                }
            }

            return databases;
        }

        public Database GetDatabase(string databaseName)
        {
            Database db = new Database(databaseName, null);
            db.Tables = getTables(db);
            setForeignKeys(db);
            return db;
        }

        public ConnectionSetup ConnectionSetup
        {
            get { return connectionSetup; }
        }
    }
}
