/*
 * 
 * StatementTableMappingPart creates the table mapping part for an import statement
 *  
 */



using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseModel;
using SQLDataImporter.DataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.StatementCreator
{
    public class StatementTableMappingPart
    {

        private TableMapping tableMapping;
        private SourceDataRow sourceDataRow;

        private DBColumn primaryKeyColumn;

        public StatementTableMappingPart(TableMapping tableMapping, SourceDataRow sourceDataRow)
        {
            this.tableMapping = tableMapping;
            this.sourceDataRow = sourceDataRow;
            this.primaryKeyColumn = tableMapping.DestinationTable.Columns.Where(c => c.IsPrimaryKey).FirstOrDefault();

            if (primaryKeyColumn == null) throw new Exception(String.Format("Table \"{0}\" has no primary key column",
                tableMapping.DestinationTable.Reference));
        }

        private string getTableVariable(TableMapping tableMapping)
        {
            StatementTableVariablePart tableVariablePart = new StatementTableVariablePart(tableMapping);
            return tableVariablePart.GetTableVariable();
        }

        public string GetTableVariablePart()
        {
            string dataPart = string.Join(", ", 
                tableMapping.ColumnMappings
                .Select(c => string.Format("{0} {1}", c.DestinationColumn.Name, 
                    c.DestinationColumn.DataType.ToString().Replace("varchar", "varchar(max)"))));

            return String.Format("DECLARE {0} TABLE ({1})\n", getTableVariable(tableMapping), dataPart);
        }

        private string getUpdatePart()
        {
            string updatePart = String.Format("UPDATE {0}", tableMapping.DestinationTable.Reference);
            string setPart = String.Format("SET {0}", getColumnSetList());
            string outputPart = getOutputPart();
            string wherePart = String.Format("WHERE {0}", getColumnWhereList());

            return string.Format("{0}\n{1}\n{2}\n{3}\n", updatePart,setPart, outputPart, wherePart);
        }

        private string getInsertPart()
        {
            string insertPart = String.Format("INSERT INTO {0} ({1})", tableMapping.DestinationTable.Reference, getInsertColumnNames());
            string outputPart = getOutputPart();
            string valuePart = String.Format("VALUES ({0})", getColumnValues());

            return string.Format("{0}\n{1}\n{2}\n", insertPart, outputPart, valuePart);
        }

        public string GetStatementBodyPart()
        {
            if (tableMapping.ImportType == TableMappingImportType.Insert)
            {
                return getInsertPart();
            }
            else
            {
                return getUpdatePart();
            }
        }

        private string getOutputPart()
        {
            string insertedPart = string.Join(", ", getAllColumnNames().Select(c => string.Format("inserted.{0}", c)));
            return String.Format("OUTPUT {0} INTO {1}({2})", insertedPart, getTableVariable(tableMapping), string.Join(", ", getAllColumnNames()));
        }

        private string[] getAllColumnNames()
        {
            return tableMapping.ColumnMappings.Select(c => c.DestinationColumn.Name).ToArray();
        }

        private string getInsertColumnNames()
        {
            return string.Join(", ", tableMapping.ColumnMappings
                .Where(c => c.DestinationColumn.IsPrimaryKey == false && c.ColumnUse == ColumnUse.Insert)
                .Select(c => c.DestinationColumn.Name));
        }

        private string getColumnValues()
        {
            return string.Join(", ", tableMapping.ColumnMappings
                .Where(c => c.DestinationColumn.IsPrimaryKey == false && c.ColumnUse == ColumnUse.Insert)
                .Select(c => getColumnMappingValue(c)));
        }


        private string getColumnSetList()
        {
            return string.Join(", ", tableMapping.ColumnMappings
                .Where(c => c.DestinationColumn != primaryKeyColumn && c.ColumnUse == ColumnUse.Set)
                .Select(c => string.Format("{0}={1}", c.DestinationColumn.Name, getColumnMappingValue(c))));
        }

        private string getColumnWhereList()
        {
            return string.Join(" and ", tableMapping.ColumnMappings
                .Where(c => c.ColumnUse == ColumnUse.Where)
                .Select(c => string.Format("{0} = {1}", c.DestinationColumn.Name, getColumnMappingValue(c))));
        }

        private string getColumnMappingValue(ColumnMapping mapping)
        {
            StatementColumnMappingPart columnMappingPart = new StatementColumnMappingPart(mapping, sourceDataRow);
            return columnMappingPart.GetColumnMappingValue();
        }


    }
}
