using NUnit.Framework;
using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseConnector;
using SQLDataImporter.DataImporter;
using SQLDataImporter.StatementCreator;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.Test
{
    [TestFixture]
    public class SQLServerDataImporterTest
    {



        [TestCase]
        [ExpectedException(typeof(SqlException))]
        public void TimeoutTest()
        {
            string hostNameDoesntExist = "testest";
            ConnectionSetup connectionSetup = new ConnectionSetup(hostNameDoesntExist, "", "", true);
            connectionSetup.Timeout = 1;

            ImportConfiguration config = new ImportConfiguration(null, connectionSetup, "", null);

            ImportStatement[] statements = new ImportStatement[0];

            SQLServerDataImporter importer = new SQLServerDataImporter(config);

            importer.ImportData(statements);
        }

        [TestCase]
        public void SuccesfulImportTest()
        {
            ImportStatement statement = new ImportStatement("declare @test table (id int) insert into @test values (0)", "0");
            SQLServerDataImporter importer = new SQLServerDataImporter(getConfig());
            ImportResult result = importer.ImportData(statement);

            SuccesfulImport import = (SuccesfulImport)result;
            Assert.AreEqual(1, import.RowsAffected);
            Assert.AreEqual(statement, import.Statement);
        }

        [TestCase]
        public void MultipImportsTest()
        {
            ImportStatement statement1 = new ImportStatement("declare @test table (id int) insert into @test values (0)", "0");
            ImportStatement statement2 = new ImportStatement("declare @test table (id int) insert into @test values (0) insert into @test values (0)", "1");
            ImportStatement statement3 = new ImportStatement("'a'", "2");
            SQLServerDataImporter importer = new SQLServerDataImporter(getConfig());

            ImportResult[] result = importer.ImportData(new ImportStatement[] { statement1, statement2, statement3 });

            SuccesfulImport import1 = (SuccesfulImport)result[0];
            SuccesfulImport import2 = (SuccesfulImport)result[1];
            UnsuccesfulImport import3 = (UnsuccesfulImport)result[2];

            Assert.AreEqual(1, import1.RowsAffected);
            Assert.AreEqual(statement1, import1.Statement);
            Assert.AreEqual(2, import2.RowsAffected);
            Assert.AreEqual(statement2, import2.Statement);
            Assert.AreEqual("Incorrect syntax near 'a'.", import3.ErrorMsg);
            Assert.AreEqual(statement3, import3.Statement);
        }

        [TestCase]
        public void UnsuccesfulImportTest()
        {
            ImportStatement statement = new ImportStatement("'a'", "0");
            SQLServerDataImporter importer = new SQLServerDataImporter(getConfig());
            ImportResult result = importer.ImportData(statement);

            UnsuccesfulImport import = (UnsuccesfulImport)result;
            Assert.AreEqual("Incorrect syntax near 'a'.", import.ErrorMsg);
            Assert.AreEqual(statement, import.Statement);
        }



        private ImportConfiguration getConfig()
        {
            string hostName = @"UNKIE\SQLExpress";
            string dbTestName = "BATEST";
            ConnectionSetup connectionSetup = new ConnectionSetup(hostName, "", "", true);
            return new ImportConfiguration(null, connectionSetup, dbTestName, null);
        }




    }
}
