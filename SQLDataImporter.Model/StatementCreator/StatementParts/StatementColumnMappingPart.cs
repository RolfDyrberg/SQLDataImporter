/*
 * 
 * StatementColumnMappingPart outputs the corresponding ColumnMappingPart of the import statement.
 * 
 */



using SQLDataImporter.Configuration;
using SQLDataImporter.DataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.StatementCreator
{
    public class StatementColumnMappingPart
    {

        private ColumnMapping columnMapping;
        private SourceDataRow sourceDataRow;

        public StatementColumnMappingPart(ColumnMapping columnMapping, SourceDataRow sourceDataRow)
        {
            this.columnMapping = columnMapping;
            this.sourceDataRow = sourceDataRow;
        }

        public string GetColumnMappingValue()
        {
            if (columnMapping is ExcelColumnMapping)
            {
                string header = ((ExcelColumnMapping)columnMapping).SourceHeader;
                SourceDataEntry entry = sourceDataRow.GetSourceDataEntry(header);
                return excelColumnMappingPart((ExcelColumnMapping)columnMapping, entry);
            }
            else if (columnMapping is TableColumnMapping)
            {
                var tableColMapping = (TableColumnMapping)columnMapping;
                StatementTableVariablePart tableVariablePart = new StatementTableVariablePart(tableColMapping.SourceTableMapping);
                string tableVariable = tableVariablePart.GetTableVariable();
                return tableColumnMappingPart(tableColMapping, tableVariable);
            }
            else if (columnMapping is LiteralColumnMapping)
            {
                return literalColumnMappingPart((LiteralColumnMapping)columnMapping);
            }
            else
            {
                return nullColumnMappingPart((NullColumnMapping)columnMapping);
            }
        }

        private string excelColumnMappingPart(ExcelColumnMapping mapping, SourceDataEntry dataEntry)
        {
            return SQLServerDataEntryTranslator.Translate(dataEntry);
        }

        private string tableColumnMappingPart(TableColumnMapping mapping, string tableVariable)
        {
            return string.Format("(SELECT TOP 1 t.{0} FROM {1} t)", mapping.SourceColumn.Name, tableVariable);
        }

        private string literalColumnMappingPart(LiteralColumnMapping mapping)
        {
            return mapping.LiteralType == LiteralType.String ? "'" + mapping.Literal.Replace("'", "''") + "'" : mapping.Literal;
        }

        private string nullColumnMappingPart(NullColumnMapping mapping)
        {
            return "NULL";
        }

    }
}
