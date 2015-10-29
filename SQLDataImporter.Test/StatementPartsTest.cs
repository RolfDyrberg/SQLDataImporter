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
    public class StatementPartsTest
    {

        private string dbName = "TestName";


        [TestCase]
        public void DBNamePartTest()
        {
            ImportConfiguration config = getTestImportConfig();
            StatementSetupPart part = new StatementSetupPart(config);

            Assert.AreEqual("USE " + dbName, part.GetDatabasePart());
        }

        [TestCase]
        public void WarningsPartTest()
        {
            ImportConfiguration config = getTestImportConfig();
            config.ErrorHandling.IgnoreWarnings = false;
            StatementSetupPart part = new StatementSetupPart(config);

            Assert.AreEqual("SET ANSI_WARNINGS ON", part.GetWarningsPart());

            config.ErrorHandling.IgnoreWarnings = true;
            Assert.AreEqual("SET ANSI_WARNINGS OFF", part.GetWarningsPart());
        }


        [TestCase]
        public void TransactionStartPartTest()
        {
            ImportConfiguration config = getTestImportConfig();
            StatementTransactionPart part = new StatementTransactionPart(config);
            config.ErrorHandling.ImportAsTransaction = true;

            Assert.AreEqual("BEGIN TRANSACTION [ImportTransaction]\nBEGIN TRY\n", part.GetTransactionStartPart());

            config.ErrorHandling.ImportAsTransaction = false;
            Assert.AreEqual("", part.GetTransactionStartPart());
        }

        [TestCase]
        public void TransactionEndPartTest()
        {
            ImportConfiguration config = getTestImportConfig();
            StatementTransactionPart part = new StatementTransactionPart(config);
            config.ErrorHandling.ImportAsTransaction = true;

            string[] parts = part.GetTransactionEndPart()
                .Split(new string[] { "\n", "\t" }, StringSplitOptions.None).Where(s => s.Length > 0).ToArray();

            Assert.AreEqual(12, parts.Length);
            Assert.AreEqual("COMMIT TRANSACTION [ImportTransaction]", parts[0]);
            Assert.AreEqual("END TRY", parts[1]);
            Assert.AreEqual("BEGIN CATCH", parts[2]);
            Assert.AreEqual("ROLLBACK TRANSACTION [ImportTransaction]", parts[3]);
            Assert.AreEqual("DECLARE @ErrorMessage NVARCHAR(MAX)", parts[4]);
            Assert.AreEqual("DECLARE @ErrorSeverity INT", parts[5]);
            Assert.AreEqual("DECLARE @ErrorState INT", parts[6]);
            Assert.AreEqual("SET @ErrorMessage = ERROR_MESSAGE()", parts[7]);
            Assert.AreEqual("SET @ErrorSeverity = ERROR_SEVERITY()", parts[8]);
            Assert.AreEqual("SET @ErrorState = ERROR_STATE()", parts[9]);
            Assert.AreEqual("RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)", parts[10]);
            Assert.AreEqual("END CATCH", parts[11]);


            config.ErrorHandling.ImportAsTransaction = false;
            Assert.AreEqual("", part.GetTransactionEndPart());
        }


        [TestCase]
        [ExpectedException(typeof(Exception))]
        public void TableMappingPartConstructorTest()
        {
            DBTable table = new DBTable("dbo", "TestTable");
            DBColumn col1 = new DBColumn(table, "TestCol1", false, DBDatatype.integer);
            DBColumn col2 = new DBColumn(table, "TestCol2", false, DBDatatype.nvarchar);
            table.Columns = new List<DBColumn>() { col1, col2 };

            ColumnMapping colMap1 = new LiteralColumnMapping("1", LiteralType.Integer, col1, ColumnUse.Exclude);
            ColumnMapping colMap2 = new LiteralColumnMapping("2", LiteralType.String, col2, ColumnUse.Insert);

            TableMapping tableMapping = new TableMapping(table, TableMappingImportType.Insert, new ColumnMapping[] { colMap1, colMap2 });

            ImportConfiguration config = getTestImportConfig();

            StatementTableMappingPart part = new StatementTableMappingPart(tableMapping, null);
        }

        [TestCase]
        public void TableVariablePartTest()
        {
            DBTable table = new DBTable("dbo", "TestTable");
            DBColumn col1 = new DBColumn(table, "TestCol1", true, DBDatatype.integer);
            DBColumn col2 = new DBColumn(table, "TestCol2", false, DBDatatype.nvarchar);
            table.Columns = new List<DBColumn>() { col1, col2 };

            ColumnMapping colMap1 = new LiteralColumnMapping("1", LiteralType.Integer, col1, ColumnUse.Exclude);
            ColumnMapping colMap2 = new LiteralColumnMapping("2", LiteralType.String, col2, ColumnUse.Insert);

            TableMapping tableMapping = new TableMapping(table, TableMappingImportType.Insert, new ColumnMapping[] { colMap1, colMap2 });


            ImportConfiguration config = getTestImportConfig();

            StatementTableMappingPart part = new StatementTableMappingPart(tableMapping, null);
            string partStatement = part.GetTableVariablePart().Replace("\n", "");

            Assert.AreEqual("DECLARE @sqlimport_table_" + tableMapping.TableMappingReference.Replace(".", "_") + " TABLE (TestCol1 integer, TestCol2 nvarchar(max))",
                partStatement);
        }

        [TestCase]
        public void TableBodyPartInsertTest()
        {
            DBTable table = new DBTable("dbo", "TestTable");
            DBColumn col1 = new DBColumn(table, "TestCol1", true, DBDatatype.integer);
            DBColumn col2 = new DBColumn(table, "TestCol2", false, DBDatatype.nvarchar);
            table.Columns = new List<DBColumn>() { col1, col2 };

            ColumnMapping colMap1 = new NullColumnMapping(col1, ColumnUse.Exclude);
            ColumnMapping colMap2 = new LiteralColumnMapping("2", LiteralType.String, col2, ColumnUse.Insert);

            TableMapping tableMapping = new TableMapping(table, TableMappingImportType.Insert, new ColumnMapping[] { colMap1, colMap2 });

            ImportConfiguration config = getTestImportConfig();

            SourceDataEntry[] entries = new SourceDataEntry[] { SourceDataEntry.CreateDataEntry("", DataType.String, "") };
            SourceDataRow[] rows = new SourceDataRow[] { new SourceDataRow(entries, "0") };
            SourceDataTable dt = new SourceDataTable(rows, new string[] { "" });

            StatementTableMappingPart part = new StatementTableMappingPart(tableMapping, dt.GetDataRow(0));

            string[] bodyParts = part.GetStatementBodyPart().Split(new string[] { "\n"}, StringSplitOptions.None).Where(s => s.Length > 0).ToArray();

            Assert.AreEqual(3, bodyParts.Length);
            Assert.AreEqual("INSERT INTO dbo.TestTable (TestCol2)", bodyParts[0]);
            Assert.AreEqual("OUTPUT inserted.TestCol1, inserted.TestCol2 INTO @sqlimport_table_" + tableMapping.TableMappingReference.Replace(".", "_") + 
                "(TestCol1, TestCol2)" , bodyParts[1]);
            Assert.AreEqual("VALUES ('2')", bodyParts[2]);
        }

        [TestCase]
        public void TableBodyPartUpdateTest()
        {
            DBTable table = new DBTable("dbo", "TestTable");
            DBColumn col1 = new DBColumn(table, "TestCol1", true, DBDatatype.integer);
            DBColumn col2 = new DBColumn(table, "TestCol2", false, DBDatatype.nvarchar);
            DBColumn col3 = new DBColumn(table, "TestCol3", false, DBDatatype.integer);
            table.Columns = new List<DBColumn>() { col1, col2, col3 };

            TableMapping sourceTablemapping = new TableMapping(new DBTable("dbo", "TestTable2"), TableMappingImportType.Insert, null);

            ColumnMapping colMap1 = new TableColumnMapping(sourceTablemapping, col1, col1, ColumnUse.Where);
            ColumnMapping colMap2 = new LiteralColumnMapping("2", LiteralType.String, col2, ColumnUse.Where);
            ColumnMapping colMap3 = new LiteralColumnMapping("3", LiteralType.String, col3, ColumnUse.Set);

            TableMapping tableMapping = new TableMapping(table, TableMappingImportType.Update, new ColumnMapping[] { colMap1, colMap2, colMap3 });

            ImportConfiguration config = getTestImportConfig();

            SourceDataEntry[] entries = new SourceDataEntry[] { SourceDataEntry.CreateDataEntry("", DataType.String, "") };
            SourceDataRow[] rows = new SourceDataRow[] { new SourceDataRow(entries, "0") };
            SourceDataTable dt = new SourceDataTable(rows, new string[] { "" });

            StatementTableMappingPart part = new StatementTableMappingPart(tableMapping, dt.GetDataRow(0));

            string[] bodyParts = part.GetStatementBodyPart().Split(new string[] { "\n" }, StringSplitOptions.None).Where(s => s.Length > 0).ToArray();

            Assert.AreEqual(4, bodyParts.Length);
            Assert.AreEqual("UPDATE dbo.TestTable", bodyParts[0]);
            Assert.AreEqual("SET TestCol3='3'", bodyParts[1]);
            Assert.AreEqual("OUTPUT inserted.TestCol1, inserted.TestCol2, inserted.TestCol3 INTO @sqlimport_table_" + 
                tableMapping.TableMappingReference.Replace(".", "_") + "(TestCol1, TestCol2, TestCol3)", bodyParts[2]);
            Assert.AreEqual("WHERE TestCol1 = (SELECT TOP 1 t.TestCol1 FROM @sqlimport_table_" +
                sourceTablemapping.TableMappingReference.Replace(".", "_") + " t) and TestCol2 = '2'", bodyParts[3]);
        }


        [TestCase]
        public void StatementColumnMappingPartTest()
        {
            DBColumn col = new DBColumn(null, "TestCol", true, DBDatatype.integer);

            SourceDataEntry entry = SourceDataEntry.CreateDataEntry("Test", DataType.String, "Test");
            SourceDataRow row = new SourceDataRow(new SourceDataEntry[] {entry}, "");

            DBTable table = new DBTable("dbo", "TestTable");
            DBColumn col1 = new DBColumn(table, "TestCol1", true, DBDatatype.integer);
            table.Columns = new List<DBColumn>() { col1 };
            TableMapping tableMapping = new TableMapping(table, TableMappingImportType.Insert, null);

            NullColumnMapping nullColumnMapping = new NullColumnMapping(col, ColumnUse.Insert);
            LiteralColumnMapping literalColumnMapping1 = new LiteralColumnMapping("Test", LiteralType.String, col, ColumnUse.Insert);
            LiteralColumnMapping literalColumnMapping2 = new LiteralColumnMapping("Test's", LiteralType.String, col, ColumnUse.Insert);
            ExcelColumnMapping excelColumnMapping = new ExcelColumnMapping("Test", col, ColumnUse.Insert);
            TableColumnMapping tableColMapping = new TableColumnMapping(tableMapping, col1, col, ColumnUse.Insert);

            StatementColumnMappingPart nullColumnPart = new StatementColumnMappingPart(nullColumnMapping, row);
            StatementColumnMappingPart literalColumnPart1 = new StatementColumnMappingPart(literalColumnMapping1, row);
            StatementColumnMappingPart literalColumnPart2 = new StatementColumnMappingPart(literalColumnMapping2, row);
            StatementColumnMappingPart excelColumnPart = new StatementColumnMappingPart(excelColumnMapping, row);
            StatementColumnMappingPart tableColumnPart = new StatementColumnMappingPart(tableColMapping, row);

            Assert.AreEqual("NULL", nullColumnPart.GetColumnMappingValue());
            Assert.AreEqual("'Test'", literalColumnPart1.GetColumnMappingValue());
            Assert.AreEqual("'Test''s'", literalColumnPart2.GetColumnMappingValue());
            Assert.AreEqual("'Test'", excelColumnPart.GetColumnMappingValue());

            StatementTableVariablePart tableVariablePart = new StatementTableVariablePart(tableMapping);
            Assert.AreEqual(String.Format("(SELECT TOP 1 t.TestCol1 FROM {0} t)", tableVariablePart.GetTableVariable()),
                tableColumnPart.GetColumnMappingValue());
        }


        private ImportConfiguration getTestImportConfig()
        {
            ErrorHandling errorHandling = new ErrorHandling();
            return new ImportConfiguration(new TableMapping[0], null, dbName, errorHandling);
        }

    }
}
