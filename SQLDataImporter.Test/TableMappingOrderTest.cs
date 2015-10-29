using NUnit.Framework;
using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseModel;
using SQLDataImporter.StatementCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.Test
{
    [TestFixture]
    public class TableMappingOrderTest
    {

        [TestCase]
        public void NoTableTest()
        {
            TableMapping[] mappingArray = new TableMapping[0];
            TableMappingOrderer tableMappingOrderer = new TableMappingOrderer(mappingArray);
            TableMapping[] order = tableMappingOrderer.OrderTableMappings();

            Assert.AreEqual(0, order.Length);
        }


        [TestCase]
        public void OneTableTest()
        {
            TableMapping[] mappingArray = new TableMapping[1] { new TableMapping(new DBTable("", ""), TableMappingImportType.Insert, new ColumnMapping[0]) };
            TableMappingOrderer tableMappingOrderer = new TableMappingOrderer(mappingArray);
            TableMapping[] order = tableMappingOrderer.OrderTableMappings();

            Assert.AreEqual(1, order.Length);
        }


        [TestCase]
        public void TwoTablesDifferentOrderTest()
        {
            TableMapping[] testMappings = TestData.TableMappingTestData();
            
            TableMapping[] mappingArray = new TableMapping[2] { testMappings[0], testMappings[2] };
            TableMappingOrderer tableMappingOrderer = new TableMappingOrderer(mappingArray);
            TableMapping[] order = tableMappingOrderer.OrderTableMappings();

            Assert.AreEqual(2, order.Length);
            Assert.AreEqual(testMappings[2].TableMappingReference, order[0].TableMappingReference);
            Assert.AreEqual(testMappings[0].TableMappingReference, order[1].TableMappingReference);
        }


        [TestCase]
        public void TwoTablesSameOrderTest()
        {
            TableMapping[] testMappings = TestData.TableMappingTestData();

            TableMapping[] mappingArray = new TableMapping[2] { testMappings[1], testMappings[3] };
            TableMappingOrderer tableMappingOrderer = new TableMappingOrderer(mappingArray);
            TableMapping[] order = tableMappingOrderer.OrderTableMappings();

            Assert.AreEqual(2, order.Length);
            Assert.AreEqual(testMappings[1].TableMappingReference, order[0].TableMappingReference);
            Assert.AreEqual(testMappings[3].TableMappingReference, order[1].TableMappingReference);
        }


        [TestCase]
        public void ThreeTablesDifferentOrderTest()
        {
            TableMapping[] testMappings = TestData.TableMappingTestData();

            TableMapping[] mappingArray = new TableMapping[3] { testMappings[0], testMappings[1], testMappings[2] };
            TableMappingOrderer tableMappingOrderer = new TableMappingOrderer(mappingArray);
            TableMapping[] order = tableMappingOrderer.OrderTableMappings();

            Assert.AreEqual(3, order.Length);
            Assert.AreEqual(testMappings[2].TableMappingReference, order[0].TableMappingReference);
            Assert.AreEqual(testMappings[0].TableMappingReference, order[1].TableMappingReference);
            Assert.AreEqual(testMappings[1].TableMappingReference, order[2].TableMappingReference);
        }


        [TestCase]
        public void FourTableTest()
        {
            TableMapping[] testMappings = TestData.TableMappingTestData();
            TableMappingOrderer tableMappingOrderer = new TableMappingOrderer(testMappings);
            TableMapping[] order = tableMappingOrderer.OrderTableMappings();

            Assert.AreEqual(4, order.Length);
            Assert.AreEqual(testMappings[2].TableMappingReference, order[0].TableMappingReference);
            Assert.AreEqual(testMappings[0].TableMappingReference, order[1].TableMappingReference);
            Assert.AreEqual(testMappings[1].TableMappingReference, order[2].TableMappingReference);
            Assert.AreEqual(testMappings[3].TableMappingReference, order[3].TableMappingReference);
        }


        [TestCase]
        [ExpectedException(typeof(ContainsCycleException))]
        public void CycleTestTwoTable()
        {
            DBTable table1 = new DBTable("dbo", "1");
            DBColumn table1ID = new DBColumn(table1, "1_id", true, DBDatatype.integer);
            table1.Columns = new List<DBColumn>() { table1ID };

            DBTable table2 = new DBTable("dbo", "2");
            DBColumn table2ID = new DBColumn(table2, "2_id", true, DBDatatype.integer);
            table2.Columns = new List<DBColumn>() { table2ID };

            TableMapping t1Mapping = new TableMapping(table1, TableMappingImportType.Insert, new ColumnMapping[0]);
            TableMapping t2Mapping = new TableMapping(table2, TableMappingImportType.Insert, new ColumnMapping[0]);

            TableColumnMapping t1ColMaping = new TableColumnMapping(t2Mapping, table2ID, table1ID, ColumnUse.Insert);
            TableColumnMapping t2ColMaping = new TableColumnMapping(t1Mapping, table1ID, table2ID, ColumnUse.Insert);

            t1Mapping.ColumnMappings = new ColumnMapping[] { t1ColMaping };
            t2Mapping.ColumnMappings = new ColumnMapping[] { t2ColMaping };

            TableMapping[] mappingArray = new TableMapping[] { t1Mapping, t2Mapping };
            TableMappingOrderer tableMappingOrderer = new TableMappingOrderer(mappingArray);
            TableMapping[] order = tableMappingOrderer.OrderTableMappings();
        }


        [TestCase]
        [ExpectedException(typeof(ContainsCycleException))]
        public void CycleTestFourTable()
        {
            DBTable table1 = new DBTable("dbo", "1");
            DBColumn table1ID = new DBColumn(table1, "1_id", true, DBDatatype.integer);
            table1.Columns = new List<DBColumn>() { table1ID };

            DBTable table2 = new DBTable("dbo", "2");
            DBColumn table2ID = new DBColumn(table2, "2_id", true, DBDatatype.integer);
            table2.Columns = new List<DBColumn>() { table2ID };

            DBTable table3 = new DBTable("dbo", "3");
            DBColumn table3ID = new DBColumn(table3, "3_id", true, DBDatatype.integer);
            table3.Columns = new List<DBColumn>() { table3ID };

            DBTable table4 = new DBTable("dbo", "4");
            DBColumn table4ID = new DBColumn(table4, "4_id", true, DBDatatype.integer);
            table4.Columns = new List<DBColumn>() { table4ID };

            TableMapping t1Mapping = new TableMapping(table1, TableMappingImportType.Insert, new ColumnMapping[0]);
            TableMapping t2Mapping = new TableMapping(table2, TableMappingImportType.Insert, new ColumnMapping[0]);
            TableMapping t3Mapping = new TableMapping(table3, TableMappingImportType.Insert, new ColumnMapping[0]);
            TableMapping t4Mapping = new TableMapping(table4, TableMappingImportType.Insert, new ColumnMapping[0]);

            TableColumnMapping t21ColMaping = new TableColumnMapping(t1Mapping, table1ID, table2ID, ColumnUse.Insert);
            TableColumnMapping t24ColMaping = new TableColumnMapping(t4Mapping, table1ID, table2ID, ColumnUse.Insert);
            TableColumnMapping t3ColMaping = new TableColumnMapping(t2Mapping, table1ID, table2ID, ColumnUse.Insert);
            TableColumnMapping t4ColMaping = new TableColumnMapping(t3Mapping, table1ID, table2ID, ColumnUse.Insert);

            t2Mapping.ColumnMappings = new ColumnMapping[] { t21ColMaping, t24ColMaping };
            t3Mapping.ColumnMappings = new ColumnMapping[] { t3ColMaping };
            t4Mapping.ColumnMappings = new ColumnMapping[] { t4ColMaping };

            TableMapping[] mappingArray = new TableMapping[] { t1Mapping, t2Mapping, t3Mapping, t4Mapping };
            TableMappingOrderer tableMappingOrderer = new TableMappingOrderer(mappingArray);
            TableMapping[] order = tableMappingOrderer.OrderTableMappings();
        }

    }
}
