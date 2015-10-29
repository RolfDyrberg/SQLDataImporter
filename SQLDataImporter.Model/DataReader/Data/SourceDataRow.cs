/*
 *
 * SourceDataRow contains the SourceDataEntries from a row in the source file
 * 
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DataReader
{
    public class SourceDataRow
    {
        private string rowReference;
        private Dictionary<string, SourceDataEntry> dataEntries;

        public SourceDataRow(SourceDataEntry[] dataEntries, string rowReference)
        {
            this.rowReference = rowReference;

            this.dataEntries = new Dictionary<string, SourceDataEntry>();

            foreach (SourceDataEntry entry in dataEntries)
            {
                this.dataEntries.Add(entry.ColumnReference, entry);
            }
        }

        public SourceDataEntry GetSourceDataEntry(string columnReference)
        {
            if (dataEntries.ContainsKey(columnReference))
            {
                return dataEntries[columnReference];
            }
            else
            {
                return SourceDataEntry.CreateDataEntry("", DataType.Null, columnReference);
            }
        }

        public string RowReference
        {
            get { return rowReference; }
        }


    }
}
