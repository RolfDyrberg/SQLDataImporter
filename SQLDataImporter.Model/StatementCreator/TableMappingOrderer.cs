/*
 * 
 * A configuration that is correctly setup can be seen as a Directed Acyclical Graph.
 * TableMappingOrderer uses this to do a topological ordering of a tablemapping array.
 * If a cycle in the graph it is not a DAG, which means that the configuration is not setup
 * correctly and an exception is therefore thrown.
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
    public class TableMappingOrderer
    {

        private TableMapping[] tableMappings;

        public TableMappingOrderer(TableMapping[] tableMappings)
        {
            this.tableMappings = tableMappings;
        } 


        public TableMapping[] OrderTableMappings()
        {

            List<TableMapping> tableRefs = tableMappings.ToList();
            List<TableMapping> tableRefOrder = new List<TableMapping>();

            while (tableRefs.Count > 0)
            {

                TableMapping toRemove = null;
                foreach (TableMapping tableMapping in tableRefs)
                {
                    List<TableMapping> sourceMappings = tableMapping.ColumnMappings
                        .Where(c => c.GetType() == typeof(TableColumnMapping))
                        .Select(c => (TableColumnMapping)c)
                        .Where(c => tableRefs.Contains(c.SourceTableMapping))
                        .Select(c => c.SourceTableMapping)
                        .ToList();

                    if (sourceMappings.Count == 0)
                    {
                        toRemove = tableMapping;
                        break;
                    }
                }

                if (toRemove == null) throw new ContainsCycleException("Cycle found in table mappings.");


                tableRefs.Remove(toRemove);
                tableRefOrder.Add(toRemove);
            }

            return tableRefOrder.ToArray();
        }

    }


    public class ContainsCycleException : Exception
    {

        public ContainsCycleException(string message) :
            base(message) { }

    }
}
