using NUnit.Framework;
using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseModel;
using SQLDataImporter.DataReader;
using SQLDataImporter.StatementCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.Test
{


    [TestFixture]
    public class StatementCreatorTest
    {


        [TestCase]
        public void StatementCreatorOrderTest()
        {

            DBTable table = new DBTable("dbo", "TestTable");
            DBColumn col1 = new DBColumn(table, "TestCol1", true, DBDatatype.integer);
            DBColumn col2 = new DBColumn(table, "TestCol2", false, DBDatatype.nvarchar);
            DBColumn col3 = new DBColumn(table, "TestCol3", false, DBDatatype.integer);
            table.Columns = new List<DBColumn>() { col1, col2, col3 };

            Database db = new Database("TestDB", new List<DBTable>() { table });

            TableMapping sourceTablemapping = new TableMapping(new DBTable("dbo", "TestTable2"), TableMappingImportType.Insert, null);

            ColumnMapping colMap1 = new TableColumnMapping(sourceTablemapping, col1, col1, ColumnUse.Where);
            ColumnMapping colMap2 = new LiteralColumnMapping("2", LiteralType.String, col2, ColumnUse.Where);
            ColumnMapping colMap3 = new LiteralColumnMapping("3", LiteralType.String, col2, ColumnUse.Set);

            TableMapping tableMapping = new TableMapping(table, TableMappingImportType.Update, new ColumnMapping[] { colMap1, colMap2, colMap3 });

            ErrorHandling errorHandling = new ErrorHandling();
            ImportConfiguration config = new ImportConfiguration(new TableMapping[] { tableMapping }, null, "TestDB", errorHandling);

            SourceDataEntry[] entries = new SourceDataEntry[] {SourceDataEntry.CreateDataEntry("", DataType.String, "") };
            SourceDataRow[] rows = new SourceDataRow[] { new SourceDataRow(entries, "0") };
            SourceDataTable dt = new SourceDataTable(rows, new string[] { "" });

            SQLServerStatementCreator statementCreator = new SQLServerStatementCreator(config, dt);

            ImportStatement statement = statementCreator.CreateStatement(0);
            ImportStatement[] statements = statementCreator.CreateStatements();

            Assert.AreEqual(1, statements.Length);
            Assert.AreEqual(statement.RowReference, statements[0].RowReference);
            Assert.AreEqual(statement.SqlStatement, statements[0].SqlStatement);

            string[] lines = statement.SqlStatement
                .Split(new string[] { "\n" }, StringSplitOptions.None).Where(s => s.Length > 0).ToArray();

            StatementSetupPart setupPart = new StatementSetupPart(config);

            Assert.AreEqual(setupPart.GetDatabasePart(), lines[0]);
            Assert.AreEqual(setupPart.GetWarningsPart(), lines[1]);


            StatementTransactionPart transPart = new StatementTransactionPart(config);
            string[] transStartPart = transPart.GetTransactionStartPart()
                .Split(new string[] { "\n" }, StringSplitOptions.None).Where(s => s.Length > 0).ToArray(); ;

            Assert.AreEqual(2, transStartPart.Length);
            Assert.AreEqual(transStartPart[0], lines[2]);
            Assert.AreEqual(transStartPart[1], lines[3]);


            StatementTableMappingPart tmParts = new StatementTableMappingPart(tableMapping, dt.GetDataRow(0));
            string variablePart = tmParts.GetTableVariablePart().Replace("\n", "");
            Assert.AreEqual(variablePart, lines[4]);

            string[] bodyParts = tmParts.GetStatementBodyPart()
                .Split(new string[] { "\n" }, StringSplitOptions.None).Where(s => s.Length > 0).ToArray();

            Assert.AreEqual(4, bodyParts.Length);
            Assert.AreEqual(bodyParts[0], lines[5]);
            Assert.AreEqual(bodyParts[1], lines[6]);
            Assert.AreEqual(bodyParts[2], lines[7]);
            Assert.AreEqual(bodyParts[3], lines[8]);


            string[] transEndPart = transPart.GetTransactionEndPart()
                .Split(new string[] { "\n" }, StringSplitOptions.None).Where(s => s.Length > 0).ToArray();

            Assert.AreEqual(12, transEndPart.Length);
            Assert.AreEqual(transEndPart[0], lines[9]);
            Assert.AreEqual(transEndPart[1], lines[10]);
            Assert.AreEqual(transEndPart[2], lines[11]);
            Assert.AreEqual(transEndPart[3], lines[12]);
            Assert.AreEqual(transEndPart[4], lines[13]);
            Assert.AreEqual(transEndPart[5], lines[14]);
            Assert.AreEqual(transEndPart[6], lines[15]);
            Assert.AreEqual(transEndPart[7], lines[16]);
            Assert.AreEqual(transEndPart[8], lines[17]);
            Assert.AreEqual(transEndPart[9], lines[18]);
            Assert.AreEqual(transEndPart[10], lines[19]);
            Assert.AreEqual(transEndPart[11], lines[20]);
        }



    }
}
