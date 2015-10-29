/*
 * 
 * SourceDataEntry holds the value of a cell in a source excel file.
 * The DataType describes the type of data a SourceDataEntry object contains.
 * 
 */


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DataReader
{
    public enum DataType { Bool, DateTime, Null, Number, String, Error };

    public class SourceDataEntry
    {

        private string rowReference;
        private string columnReference;

        private string value;
        private DataType dataType;

        public string Value
        { 
            get { return value; }
        }
        public DataType DataType 
        {
            get { return dataType; }
        }

        public string ColumnReference
        {
            get { return columnReference; }
        }

        private SourceDataEntry(string value, DataType dataType, string columnReference)
        {
            this.value = value;
            this.dataType = dataType;
            this.columnReference = columnReference;
        }

        public static SourceDataEntry CreateDataEntry(string value, DataType dataType, string columnReference)
        {
            if (dataType == DataType.Null)
            {
                return new SourceDataEntry("NULL", dataType, columnReference);
            }
            else if (dataType == DataType.Bool)
            {
                if (value != "1" && value != "0") throw new Exception("Value of Bool data type must be '1' or '0'");
                return new SourceDataEntry(value, dataType, columnReference);
            }
            else if (dataType == DataType.DateTime)
            {
                DateTime d;
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                {
                    return new SourceDataEntry(value, dataType, columnReference);
                }
                else
                {
                    throw new Exception("Could not parse datetime");
                }
            }
            else  if (dataType == DataType.Number)
            {
                double v;
                if (Double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out v))
                {
                    return new SourceDataEntry(value, dataType, columnReference);
                }
                else
                {
                    throw new Exception("Could not parse double value");
                }
            }
            else
            {
                return new SourceDataEntry(value, dataType, columnReference);
            }
        }

    }

    


    

}
