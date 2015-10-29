using NUnit.Framework;
using SQLDataImporter.DatabaseConnector;
using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.Test
{
    [TestFixture]
    public class SQLServerConnectorTest
    {

        private string hostName = @"UNKIE\SQLExpress";
        private string dbTestName = "BATEST";

        [TestCase]
        [ExpectedException(typeof(SqlException))]
        public void TimeoutTest()
        {
            string hostNameDoesntExist = "testest";
            ConnectionSetup connectionSetup = new ConnectionSetup(hostNameDoesntExist, "", "", true);
            connectionSetup.Timeout = 1;
            SQLServerConnector conn = new SQLServerConnector(connectionSetup);
            
            conn.GetDatabaseNames();
        }

        [TestCase]
        public void WindowsLoginTest()
        {
            ConnectionSetup connectionSetup = new ConnectionSetup(hostName, "", "", true);
            connectionSetup.Timeout = 1;
            SQLServerConnector conn = new SQLServerConnector(connectionSetup);
            
            Assert.IsNotEmpty(conn.GetDatabaseNames());
        }

        [TestCase]
        public void NonWindowsLoginTest()
        {
            ConnectionSetup connectionSetup = new ConnectionSetup(hostName, "TestUser", "TestPass", false);
            connectionSetup.Timeout = 1;
            SQLServerConnector conn = new SQLServerConnector(connectionSetup);

            Assert.IsNotEmpty(conn.GetDatabaseNames());
        }


        [TestCase]
        public void DatabaseExistsTest()
        {
            ConnectionSetup connectionSetup = new ConnectionSetup(hostName, "", "", true);
            connectionSetup.Timeout = 1;
            SQLServerConnector conn = new SQLServerConnector(connectionSetup);

            Assert.Contains(dbTestName, conn.GetDatabaseNames().ToList());
        }

        [TestCase]
        public void TablesExistsTest()
        {
            ConnectionSetup connectionSetup = new ConnectionSetup(hostName, "", "", true);
            connectionSetup.Timeout = 1;
            SQLServerConnector conn = new SQLServerConnector(connectionSetup);

            Database db = conn.GetDatabase(dbTestName);
            
            string[] tableNames = db.Tables.Select(t => t.Name).ToArray();

            Assert.AreEqual(4, db.Tables.Count);
            Assert.Contains("Address", tableNames);
            Assert.Contains("Person", tableNames);
            Assert.Contains("ContactInfo", tableNames);
            Assert.Contains("ContactInfoType", tableNames);
        }

        [TestCase]
        public void ColumnsExistsTest()
        {
            ConnectionSetup connectionSetup = new ConnectionSetup(hostName, "", "", true);
            connectionSetup.Timeout = 1;
            SQLServerConnector conn = new SQLServerConnector(connectionSetup);

            Database db = conn.GetDatabase(dbTestName);
            DBTable personTable = db.Tables.Where(t => t.Name == "Person").First();
            string[] columnNames = personTable.Columns.Select(c => c.Name).ToArray();

            Assert.Contains("p_id", columnNames);
            Assert.Contains("FirstName", columnNames);
            Assert.Contains("LastName", columnNames);
            Assert.Contains("a_id", columnNames);
        }



    }
}
