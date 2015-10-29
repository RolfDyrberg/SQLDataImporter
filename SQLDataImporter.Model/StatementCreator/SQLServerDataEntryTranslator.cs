/*
 * 
 * SQLServerDataEntryTranslator translates values from the SourceDataEntries
 * into a value string that can be used in sql statements.
 * 
 */



using SQLDataImporter.DataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.StatementCreator
{

    public class SQLServerDataEntryTranslator
    {

        public static string Translate(SourceDataEntry dataEntry)
        {
            return sqlStringValueFromDataEntry(dataEntry);
        }

        private static string sqlStringValueFromDataEntry(SourceDataEntry dataEntry)
        {
            switch (dataEntry.DataType)
            {
                case DataType.Bool:
                    return dataEntry.Value;
                case DataType.DateTime:
                    return "'" + dataEntry.Value + "'";
                case DataType.Error:
                    return "NULL"; // TODO: throw exception instead?
                case DataType.Null:
                    return "NULL";
                case DataType.Number:
                    return dataEntry.Value;
                default:
                    return "'" + dataEntry.Value.Replace("'", "''") + "'";
            }
        }

    }
}
