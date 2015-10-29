/*
 * 
 * SQLServerStatementCreator is used to create ImportStatments from SourceDataRows based on an ImportConfiguration
 * 
 */


using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseModel;
using SQLDataImporter.DataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.StatementCreator
{
    public class SQLServerStatementCreator
    {
       
        private ImportConfiguration config;
        private SourceDataTable dataTable;

        public SQLServerStatementCreator(ImportConfiguration config, SourceDataTable dataTable)
        {
            this.config = config;
            this.dataTable = dataTable;
        }

        public ImportStatement[] CreateStatements()
        {
            return generatInsertSQL();
        }

        public ImportStatement CreateStatement(int row)
        {
            return createInsertStatement(dataTable.GetDataRow(row));
        }


        private ImportStatement[] generatInsertSQL()
        {
            List<ImportStatement> statements = new List<ImportStatement>();
            
            for (int row = 0; row < dataTable.NumberOfRows; row++)
            {                
                ImportStatement insertStatement = createInsertStatement(dataTable.GetDataRow(row));
                statements.Add(insertStatement);
            }

            return statements.ToArray();
        }

        private ImportStatement createInsertStatement(SourceDataRow sourceDataRow)
        {

            TableMappingOrderer tableMappingOrderer = new TableMappingOrderer(config.TableMappings);
            TableMapping[] orderedTableMappings = tableMappingOrderer.OrderTableMappings();

            StatementSetupPart statementSetupPart = new StatementSetupPart(config);
            string setupPart = string.Format("{0}\n{1}", statementSetupPart.GetDatabasePart(), statementSetupPart.GetWarningsPart());

            StatementTransactionPart statementTransactionPart = new StatementTransactionPart(config);
            string transactionStartPart = statementTransactionPart.GetTransactionStartPart();
            string transactionEndPart = statementTransactionPart.GetTransactionEndPart();

            List<string> statementTableVariableParts = new List<string>();
            List<string> statmentBodyParts = new List<string>();

            foreach (TableMapping tableMapping in orderedTableMappings)
            {
                StatementTableMappingPart statementTableMappingPart = new StatementTableMappingPart(tableMapping, sourceDataRow);
                statementTableVariableParts.Add(statementTableMappingPart.GetTableVariablePart());
                statmentBodyParts.Add(statementTableMappingPart.GetStatementBodyPart());
            }

            string importStatementStructure = "{0}\n{1}\n{2}\n{3}\n{4}";

            string importStatement = string.Format(importStatementStructure, setupPart, transactionStartPart, 
                string.Join("\n", statementTableVariableParts), string.Join("\n", statmentBodyParts), transactionEndPart);

            return new ImportStatement(importStatement, sourceDataRow.RowReference);
        }

    }
}
