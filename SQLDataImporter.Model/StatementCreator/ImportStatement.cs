/*
 * 
 * ImportStatement holds the SQL code generated for each row of source data
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.StatementCreator
{
    public class ImportStatement
    {

        private string sqlStatement;
        private string rowReference;


        public ImportStatement(string sqlStatement, string rowReference)
        {
            this.sqlStatement = sqlStatement;
            this.rowReference = rowReference;
        }

        public string SqlStatement
        {
            get { return sqlStatement; }
        }

        public string RowReference
        {
            get { return rowReference; }
        }
        
    }
}
