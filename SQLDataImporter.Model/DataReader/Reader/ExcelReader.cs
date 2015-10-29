/*
 * 
 * ExcelReader reads the contents an excel file into a SourceDataTable object.
 * Row references correspond to row numbers in the selected worksheet.
 * Column references correspond to column names in the selected worksheet.
 * 
 */




using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLDataImporter.DataReader
{
    public class ExcelReader : IDataReader
    {

        private string path;
        private string selectedWorksheetName;
        private string[] worksheetNames;

        private bool hasHeaders;
        
        public ExcelReader(string path)
        {
            this.path = path;
            this.hasHeaders = true;

            setupFile(path);
        }


        private void setupFile(string path)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(path, false))
            {
                WorkbookPart workbookPart = document.WorkbookPart;

                worksheetNames = getWorksheetNames(workbookPart);
                    
                Sheet selectedSheet = getSheetByName(workbookPart, worksheetNames[0]);                  
                selectedWorksheetName = selectedSheet.Name;
            }
        }

        private WorksheetPart getWorksheetPartById(WorkbookPart workbookPart, string Id)
        {
            WorksheetPart worksheetPart = (WorksheetPart)(workbookPart.GetPartById(Id));

            return worksheetPart;
        }

        private Sheet getSheetByName(WorkbookPart workbookPart, string sheetName)
        {
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();

            if (sheet == null)
            {
                throw new ArgumentException("Couldn't find sheet: " + sheetName);
            }

            return sheet;
        }

        private string[] getWorksheetNames(WorkbookPart workbookPart)
        {
            List<string> sheetNames = new List<string>();
            foreach (Sheet s in workbookPart.Workbook.Sheets)
            {
                sheetNames.Add(s.Name);
            }
            return sheetNames.ToArray();
        }


        private int getRowNumber(string cellReference)
        {
            Regex regex = new Regex("[0-9]+");
            Match match = regex.Match(cellReference);
            return int.Parse(match.Value);
        }

        private string getColumnName(string cellReference)
        {
            Regex regex = new Regex("[A-Z]+", RegexOptions.IgnoreCase);
            Match match = regex.Match(cellReference);
            return match.Value;
        }

        private string getCellValue(WorkbookPart workbookPart, Cell cell)
        {
            string value = null;

            if (cell.DataType != null)
            {
                switch (cell.DataType.Value)
                {

                    case CellValues.Boolean:
                        value = cell.InnerText == "0" ? "FALSE" : "TRUE";
                        break;

                    case CellValues.SharedString:
                        SharedStringTablePart stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                        value = stringTable.SharedStringTable.ElementAt(int.Parse(cell.InnerText)).InnerText;
                        break;

                    default:
                        value = cell.InnerText;
                        break;

                }
            }
            return value;
        }

        private SourceDataEntry getcellDataEntry(WorkbookPart workbookPart, Cell cell, SortedDictionary<string, string> headers)
        {
            DataType dataType = DataType.Null;

            string value = cell.InnerText;
            if (value.Length > 0)
            {
                dataType = DataType.Number;
            }
            
            if (cell.DataType != null)
            {
                switch (cell.DataType.Value)
                {
                    case CellValues.Boolean:
                        value = cell.CellValue.ToString() == "0" ? "0" : "1";
                        dataType = DataType.Bool;
                        break;
                    case CellValues.SharedString:
                        SharedStringTablePart stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                        value = stringTable.SharedStringTable.ElementAt(int.Parse(cell.InnerText)).InnerText;
                        dataType = DataType.String;
                        break;
                    case CellValues.InlineString:
                        value = cell.InnerText;
                        dataType = DataType.String;
                        break;
                    case CellValues.String:
                        value = cell.CellValue.InnerText;
                        dataType = DataType.String;
                        break;
                    case CellValues.Error:
                        value = "";
                        dataType = DataType.Error;
                        break;
                    default:
                        break;
                }
            }
            else if (cell.StyleIndex != null && 
                styleIndexIsDate(workbookPart.WorkbookStylesPart.Stylesheet, (int)cell.StyleIndex.Value))
            {
                double d = Double.Parse(value, CultureInfo.InvariantCulture);
                value = DateTime.FromOADate(d).ToString(CultureInfo.InvariantCulture);
                dataType = DataType.DateTime;
            }
            else if (cell.CellFormula != null)
            {
                double v;
                if (Double.TryParse(cell.CellValue.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out v))
                {
                    value = v.ToString(CultureInfo.InvariantCulture);
                    dataType = DataType.Number;
                }
                else
                {
                    value = cell.CellValue.InnerText;
                    dataType = DataType.String;
                }
            }

            string columnName = getColumnName(cell.CellReference);
            string columnReference = headers.ContainsKey(columnName) ? headers[getColumnName(cell.CellReference)] : columnName;
            return SourceDataEntry.CreateDataEntry(value, dataType, columnReference);
        }

        private bool styleIndexIsDate(Stylesheet stylesheet, int styleIndex)
        {
            var cellFormat = (CellFormat)stylesheet.CellFormats.ElementAt(styleIndex);
            int numberFormatId = (int)cellFormat.NumberFormatId.Value;

            // The number formats that indicate a date
            // For this magical list see: http://stackoverflow.com/a/23213725
            List<int> dateTimeFormat = new List<int> { 14, 22, 37, 38, 39, 40, 47, 55 };

            return dateTimeFormat.Contains(numberFormatId);
        }


        public string SelectedWorksheetName
        {
            get
            {
                return selectedWorksheetName;
            }
            set
            {
                selectedWorksheetName = value;
            }
        }

        public string[] WorkSheetNames
        {
            get
            {
                return worksheetNames;
            }
            private set 
            { 
                worksheetNames = value; 
            }            
        }

        public bool HasHeaders
        {
            get
            {
                return hasHeaders;
            }
            set
            {
                hasHeaders = value;
            }
        }

        override public SourceDataTable ReadToDataTable()
        {
            return getDataTable(getDataRows(false));
        }


        public override SourceDataTable ReadFirstRowToDataTable()
        {
            List<SourceDataRow> dataRows = getDataRows(true);
            return getDataTable(dataRows);
        }


        override public string[] GetHeaderNames()
        {
            return getHeaders().Values.ToArray();
        }

        private SourceDataTable getDataTable(List<SourceDataRow> dataRows)
        {
            return new SourceDataTable(dataRows.ToArray(), GetHeaderNames());
        }

        private bool entryContainsValue(SourceDataEntry entry)
        {
            return entry.Value.Length > 0 || entry.DataType != DataType.Null ||entry.DataType != DataType.Error;
        }


        private List<SourceDataRow> getDataRows(bool onlyFirstRow)
        {
            List<SourceDataRow> rowList = new List<SourceDataRow>();
            SortedDictionary<string, string> headers = getHeaders();

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(path, false))
            {
                WorkbookPart workbookPart = document.WorkbookPart;
                Sheet sheet = getSheetByName(workbookPart, selectedWorksheetName);

                WorksheetPart worksheetPart = getWorksheetPartById(workbookPart, sheet.Id);

                IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>();

                int rowsToSkip = hasHeaders ? 1 : 0;

                foreach (Row row in rows.Skip(rowsToSkip))
                {
                    List<SourceDataEntry> rowEntries = new List<SourceDataEntry>();

                    bool rowContainsData = false;

                    string rowReference = row.RowIndex;

                    foreach (Cell cell in row.Descendants<Cell>())
                    {
                        SourceDataEntry cellDataEntry = getcellDataEntry(workbookPart, cell, headers);
                        rowEntries.Add(cellDataEntry);

                        if (entryContainsValue(cellDataEntry)) rowContainsData = true;
                    }

                    if (rowContainsData)
                    {
                        rowList.Add(new SourceDataRow(rowEntries.ToArray(), rowReference));
                    }

                    if (onlyFirstRow && rowContainsData) break;
                }
            }

            return rowList;
        }

        private SortedDictionary<string, string> getHeaders()
        {
            SortedDictionary<string, string> headers = new SortedDictionary<string, string>();

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(path, false))
            {
                WorkbookPart workbookPart = document.WorkbookPart;
                Sheet sheet = getSheetByName(workbookPart, selectedWorksheetName);

                WorksheetPart worksheetPart = getWorksheetPartById(workbookPart, sheet.Id);
                Row headerRow = worksheetPart.Worksheet.Descendants<Row>().First();

                foreach (Cell c in headerRow.Descendants<Cell>())
                {
                    string cellColumn = getColumnName(c.CellReference);
                    string header = getCellValue(workbookPart, c) != null ? getCellValue(workbookPart, c) : cellColumn;

                    headers.Add(cellColumn, header);
                }
            }

            return headers;
        }
    }

}
