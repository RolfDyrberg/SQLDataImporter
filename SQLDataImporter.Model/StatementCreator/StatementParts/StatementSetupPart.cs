/*
 * 
 * StatementSetupPart generates the SQL that provides the basic setup for an ImportStatement
 * 
 */

using SQLDataImporter.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.StatementCreator
{
    public class StatementSetupPart
    {
        private ImportConfiguration config;
        private ErrorHandling errorHandling;

        public StatementSetupPart(ImportConfiguration config)
        {
            this.config = config;
            errorHandling = config.ErrorHandling;
        }

        public string GetDatabasePart()
        {
           return String.Format("USE {0}", config.DatabaseName);
        }

        public string GetWarningsPart()
        {
            return String.Format("SET ANSI_WARNINGS {0}", errorHandling.IgnoreWarnings ? "OFF" : "ON");
        }

    }
}
