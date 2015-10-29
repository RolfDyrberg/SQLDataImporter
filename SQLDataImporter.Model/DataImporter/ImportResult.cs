/*
 * 
 * ImportResult describes the result of an import and the row it was imported from.
 * This row reference should match the row reference of a SourceDataEntry and ImportStatement.
 * If the result was succesful SuccesfulImport is used.
 * If not UnsuccesfulImport is used, and the assiocated error message is passed.
 * 
 */

using SQLDataImporter.StatementCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DataImporter
{
    public abstract class ImportResult
    {

        private ImportStatement statement;

        public ImportResult(ImportStatement statement)
        {
            this.statement = statement;
        }

        public ImportStatement Statement
        {
            get { return statement; }
        }

    }


    public class SuccesfulImport : ImportResult
    {

        private int rowsAffected;

        public SuccesfulImport(ImportStatement statement, int rowsAffected)
            : base(statement)
        {
            this.rowsAffected = rowsAffected;
        }

        public int RowsAffected
        {
            get { return rowsAffected; }
        }
    }

    public class UnsuccesfulImport : ImportResult
    {

        private string errorMsg;

        public UnsuccesfulImport(ImportStatement statement, string errorMsg)
            : base(statement)
        {
            this.errorMsg = errorMsg;
        }

        public string ErrorMsg
        {
            get { return errorMsg; }
        }

    }
}
