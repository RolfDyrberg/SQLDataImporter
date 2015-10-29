/*
 * 
 * SourceDataTable contains SourceDataRows and headers for each column.
 * The data entries are read from an excel file and the datatable is later read to generate 
 * data for sql imports statements.
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DataReader
{
    public class SourceDataTable
    {

        private SourceDataRow[] rows;
        private string[] headers;

        public string[] Headers
        { 
            get { return headers; } 
            private set { this.headers = value; } 
        }

        public int NumberOfRows
        {
            get { return rows.Length; }
        }

        public SourceDataTable(SourceDataRow[] rows, string[] headers)
        {
            this.rows = rows;
            this.headers = headers;
        }

        public SourceDataRow GetDataRow(int row)
        {
            return rows[row];
        }

    }
}
