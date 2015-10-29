using NUnit.Framework;
using SQLDataImporter.DataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.Test
{
    [TestFixture]
    public class ExcelReaderTest
    {

        string fileNotFound = @"ExcelTestFiles\FileNotFound.xlsx";
        string singleEmptyWorksheetFile = @"ExcelTestFiles\SingleEmptyWorksheet.xlsx";
        string twoEmptyWorksheetFile = @"ExcelTestFiles\TwoEmptyWorksheets.xlsx";
        string worksheetWithHeaders = @"ExcelTestFiles\WorksheetWithHeaders.xlsx";
        string worksheetNoHeaders = @"ExcelTestFiles\WorksheetNoHeaders.xlsx";
        string worksheetMissingHeader = @"ExcelTestFiles\WorksheetMissingHeader.xlsx";
        string worksheetEmptyCell = @"ExcelTestFiles\WorksheetWithEmptyCell.xlsx";
        string worksheetEmptyRow = @"ExcelTestFiles\WorksheetWithEmptyRow.xlsx";
        string worksheetEmptyRowInTable = @"ExcelTestFiles\WorksheetWithEmptyRowInTable.xlsx";
        string worksheetWorksheetOffset = @"ExcelTestFiles\WorksheetOffset.xlsx";
        string dataTypesTestFile = @"ExcelTestFiles\DataTypesTestFile.xlsx";
        

        [TestCase]
        [ExpectedException(typeof(FileNotFoundException))]
        public void FileNotFoundTest()
        {
            ExcelReader reader = new ExcelReader(fileNotFound);
        }

        [TestCase]
        public void EmptyWorksheetTest()
        {
            ExcelReader reader = new ExcelReader(singleEmptyWorksheetFile);
            Assert.AreEqual(1, reader.WorkSheetNames.Count());
            Assert.AreEqual("TestWorksheet", reader.WorkSheetNames[0]);
        }

        [TestCase]
        public void TwoWorkSheetsTest()
        {
            ExcelReader reader = new ExcelReader(twoEmptyWorksheetFile);
            Assert.AreEqual(2, reader.WorkSheetNames.Count());
            Assert.AreEqual("TestWorksheet1", reader.WorkSheetNames[0]);
            Assert.AreEqual("TestWorksheet2", reader.WorkSheetNames[1]);
        }

        [TestCase]
        public void WorksheetWithHeadersTest()
        {
            ExcelReader reader = new ExcelReader(worksheetWithHeaders);
            reader.HasHeaders = true;
            SourceDataTable dataTable = reader.ReadToDataTable();
            string[] headers = dataTable.Headers;

            Assert.AreEqual("TestHeader1", headers[0]);
            Assert.AreEqual("TestHeader2", headers[1]);
            Assert.AreEqual("TestHeader3", headers[2]);

            Assert.AreEqual(4, dataTable.NumberOfRows);
        }

        [TestCase]
        public void WorksheetNoHeadersTest()
        {
            ExcelReader reader = new ExcelReader(worksheetNoHeaders);
            reader.HasHeaders = false;
            SourceDataTable dataTable = reader.ReadToDataTable();
            string[] headers = dataTable.Headers;

            Assert.AreEqual("A", headers[0]);
            Assert.AreEqual("B", headers[1]);
            Assert.AreEqual("C", headers[2]);

            Assert.AreEqual(4, dataTable.NumberOfRows);
        }

        [TestCase]
        public void WorksheetMissingHeaderTest()
        {
            ExcelReader reader = new ExcelReader(worksheetMissingHeader);
            reader.HasHeaders = true;
            SourceDataTable dataTable = reader.ReadToDataTable();
            string[] headers = dataTable.Headers;

            Assert.AreEqual("TestHeader1", headers[0]);
            Assert.AreEqual("TestHeader3", headers[1]);

            Assert.AreEqual(2, headers.Length);
            Assert.AreEqual(3, dataTable.NumberOfRows);
        }

        [TestCase]
        public void WorksheetWithEmptyCellTest()
        {
            ExcelReader reader = new ExcelReader(worksheetEmptyCell);
            reader.HasHeaders = true;
            SourceDataTable dataTable = reader.ReadToDataTable();
            
            Assert.AreEqual(4, dataTable.NumberOfRows);

            string header = reader.GetHeaderNames()[1];
            SourceDataEntry emptyDataEntry = dataTable.GetDataRow(1).GetSourceDataEntry(header);

            Assert.AreEqual(DataReader.DataType.Null, emptyDataEntry.DataType);
            Assert.AreEqual("NULL", emptyDataEntry.Value);
        }

        [TestCase]
        public void WorksheetWithOffsetTest()
        {
            ExcelReader reader = new ExcelReader(worksheetWorksheetOffset);
            reader.HasHeaders = true;
            SourceDataTable dataTable = reader.ReadToDataTable();

            Assert.AreEqual(4, dataTable.NumberOfRows);
        }

        [TestCase]
        public void WorksheetWithEmptyRowTest()
        {
            ExcelReader reader = new ExcelReader(worksheetEmptyRow);
            reader.HasHeaders = true;
            SourceDataTable dataTable = reader.ReadToDataTable();

            Assert.AreEqual(3, dataTable.NumberOfRows);
        }

        [TestCase]
        public void WorksheetWithEmptyRowInTableTest()
        {
            ExcelReader reader = new ExcelReader(worksheetEmptyRowInTable);
            reader.HasHeaders = true;
            SourceDataTable dataTable = reader.ReadToDataTable();

            Assert.AreEqual(3, dataTable.NumberOfRows);
        }


        [TestCase]
        public void CellDataTypesTest()
        {
            ExcelReader reader = new ExcelReader(dataTypesTestFile);
            reader.HasHeaders = true;
            SourceDataTable dataTable = reader.ReadToDataTable();

            string[] headers = reader.GetHeaderNames();

            SourceDataEntry entry1 = dataTable.GetDataRow(0).GetSourceDataEntry(headers[0]);
            Assert.AreEqual(DataType.Bool, entry1.DataType);
            Assert.AreEqual("1", entry1.Value);

            SourceDataEntry entry2 = dataTable.GetDataRow(0).GetSourceDataEntry(headers[1]);
            Assert.AreEqual(DataType.DateTime, entry2.DataType);
            Assert.AreEqual("12/30/1899 00:00:00", entry2.Value);

            SourceDataEntry entry3 = dataTable.GetDataRow(0).GetSourceDataEntry(headers[2]);
            Assert.AreEqual(DataType.Error, entry3.DataType);
            Assert.AreEqual("", entry3.Value);

            SourceDataEntry entry4 = dataTable.GetDataRow(0).GetSourceDataEntry(headers[3]);
            Assert.AreEqual(DataType.String, entry4.DataType);
            Assert.AreEqual("Test string", entry4.Value);

            SourceDataEntry entry5 = dataTable.GetDataRow(0).GetSourceDataEntry(headers[4]);
            Assert.AreEqual(DataType.Number, entry5.DataType);
            Assert.AreEqual("1.23", entry5.Value);

            SourceDataEntry entry6 = dataTable.GetDataRow(0).GetSourceDataEntry(headers[5]);
            Assert.AreEqual(DataType.String, entry6.DataType);
            Assert.AreEqual("Test string", entry6.Value);

            SourceDataEntry entry7 = dataTable.GetDataRow(0).GetSourceDataEntry(headers[6]);
            Assert.AreEqual(DataType.String, entry7.DataType);
            Assert.AreEqual("Test string", entry7.Value);

            SourceDataEntry entry8 = dataTable.GetDataRow(0).GetSourceDataEntry(headers[7]);
            Assert.AreEqual(DataType.Number, entry8.DataType);
            Assert.AreEqual("1.23", entry8.Value);
        }



    }
}
