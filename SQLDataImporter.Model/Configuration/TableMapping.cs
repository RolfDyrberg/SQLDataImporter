/*
 * 
 * TableMapping is used to describe how data is mapped to a table in a database.
 * Each column is mapped in the ColumnMappings array.
 * 
 * To distinguish different TableMappings wich map to the same table each tablemapping has an index
 * 
 */


using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SQLDataImporter.Configuration
{

    public enum TableMappingImportType { Insert, Update };

    public class TableMapping
    {

        private static int counter = 0;

        private DBTable destinationTable;
        private int index;

        private TableMappingImportType importType;

        ColumnMapping[] columnMappings;

        public TableMapping(DBTable destinationTable, TableMappingImportType importType, ColumnMapping[] mappings)
        {
            this.destinationTable = destinationTable;
            this.importType = importType;
            this.columnMappings = mappings;
            this.index = counter;
            counter++;
        }

        public DBTable DestinationTable
        {
            get { return destinationTable; }
            set
            { 
                destinationTable = value; 

            }
        }

        public TableMappingImportType ImportType
        {
            get { return importType; }
            set { importType = value; }
        }


        public int Index
        {
            get { return index; }
        }


        public string TableMappingReference
        {
            get { return destinationTable.Reference + "_" + index; }
        }

        public ColumnMapping[] ColumnMappings
        {
            get { return columnMappings; }
            set { columnMappings = value; }
        }

        public ColumnUse[] AllowedColumnUses()
        {
            if (importType == TableMappingImportType.Update)
            {
                return new ColumnUse[] { ColumnUse.Set, ColumnUse.Where, ColumnUse.Exclude };
            }
            else
            {
                return new ColumnUse[] { ColumnUse.Insert, ColumnUse.Exclude };
            }
        }


    }
}
