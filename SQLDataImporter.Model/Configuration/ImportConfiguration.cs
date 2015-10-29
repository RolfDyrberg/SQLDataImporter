/*
 * 
 * ImportConfiguration contains all the information about how an import is to be run.
 * It contains TableMappings, a ConnectionSetup, a database name string and an ErrorHandling
 * 
 */


using SQLDataImporter.DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDataImporter.Configuration
{

    public class ImportConfiguration
    {

        private TableMapping[] tableMappings;
        private ConnectionSetup connectionSetup;

        private string databaseName;

        private ErrorHandling errorHandling;


        public ImportConfiguration(TableMapping[] tableMappings, ConnectionSetup connectionSetup, string databaseName, ErrorHandling errorHandling)
        {
            this.tableMappings = tableMappings;
            this.connectionSetup = connectionSetup;
            this.databaseName = databaseName;
            this.errorHandling = errorHandling;
        }

        public TableMapping[] TableMappings
        {
            get { return tableMappings; }
            set { tableMappings = value; }
        }

        public ConnectionSetup ConnectionSetup
        {
            get { return connectionSetup; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public ErrorHandling ErrorHandling
        {
            get { return errorHandling; }
            set { errorHandling = value; }
        }


    }
}
