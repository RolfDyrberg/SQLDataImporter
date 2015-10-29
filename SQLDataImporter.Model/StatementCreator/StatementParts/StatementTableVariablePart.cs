/*
 * 
 * StatementTableVariablePart proivdes an variable for a temporary table used in the import
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
    public class StatementTableVariablePart
    {

        private TableMapping tableMapping;

        public StatementTableVariablePart(TableMapping tableMapping)
        {
            this.tableMapping = tableMapping;
        }

        public string GetTableVariable()
        {
            return String.Format("@sqlimport_table_{0}", tableMapping.TableMappingReference.Replace('.', '_'));
        }

    }
}
