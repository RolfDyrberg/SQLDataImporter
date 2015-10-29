/*
 * 
 * ConfigurationLoader loads an xml file containing an ImportConfiguration
 * 
 */

using SQLDataImporter.DatabaseConnector;
using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SQLDataImporter.Configuration
{
    public class ConfigurationLoader
    {

        private string filePath;
        public ConfigurationLoader(string filePath)
        {
            this.filePath = filePath;
        }


        public ImportConfiguration Load()
        {
            ImportConfiguration config = null;

            using (XmlReader reader = XmlReader.Create(filePath))
            {

                ConnectionSetup connectionSetup = readConnectionSetup(reader);

                string databaseName = readDatabaseName(reader);

                ErrorHandling errorHandling = readErrorHandling(reader);

                SQLServerConnector dbConnector = new SQLServerConnector(connectionSetup);
                Database database = dbConnector.GetDatabase(databaseName);

                TableMapping[] tableMappings = readTableMappings(reader, database);

                config = new ImportConfiguration(tableMappings, connectionSetup, databaseName, errorHandling);
            }

            return config;
        }

        private ErrorHandling readErrorHandling(XmlReader reader)
        {
            ErrorHandling errorHandling = null;

            if (reader.ReadToFollowing("ErrorHandling"))
            {
                bool ignoreWarnings = bool.Parse(reader.GetAttribute("ignoreWarnings"));
                bool outputRowNumbers = bool.Parse(reader.GetAttribute("outputRowNumbers"));
                bool importAsTransaction = bool.Parse(reader.GetAttribute("importAsTransaction"));

                errorHandling = new ErrorHandling(ignoreWarnings, outputRowNumbers, importAsTransaction);
            }

            return errorHandling;
        }


        private TableMapping[] readTableMappings(XmlReader reader, Database database)
        {

            Dictionary<int, TableMapping> tableMappings = new Dictionary<int, TableMapping>();
            Dictionary<TableMapping, XElement> columnMappingReaders = new Dictionary<TableMapping, XElement>();

            if (reader.ReadToFollowing("TableMappings"))
            {

                using (XmlReader tmlReader = reader.ReadSubtree())
                {


                    while (tmlReader.ReadToFollowing("TableMapping"))
                    {
                        {
                            int index = int.Parse(tmlReader.GetAttribute("index"));
                            string destinationTableReference = tmlReader.GetAttribute("destinationTableReference");
                            string importType = tmlReader.GetAttribute("importType");

                            DBTable table = database.Tables.Where(t => t.Reference == destinationTableReference).First();

                            TableMapping tableMapping = new TableMapping(table,
                                (TableMappingImportType)Enum.Parse(typeof(TableMappingImportType), importType), null);

                            tableMappings.Add(index, tableMapping);

                            if (tmlReader.ReadToFollowing("ColumnMappings"))
                            {
                                XmlReader cmlReader = tmlReader.ReadSubtree();
                                XElement cml = XElement.Load(cmlReader);

                                columnMappingReaders.Add(tableMapping, cml);
                            }
                        }   
                    }

                    foreach (TableMapping tableMapping in columnMappingReaders.Keys)
                    {
                        List<ColumnMapping> columnMappings = new List<ColumnMapping>();

                        XElement columnMappingsElement = columnMappingReaders[tableMapping];
                        IEnumerable<XElement> columnMappingElements = columnMappingsElement.Elements();

                        foreach (XElement columnMappingElement in columnMappingElements)
                        {

                            if (columnMappingElement.Name == "ColumnMapping")
                            {

                                string type = columnMappingElement.Attribute("type").Value;
                                ColumnUse columnUse = (ColumnUse)Enum.Parse(typeof(ColumnUse), columnMappingElement.Attribute("columnUse").Value);

                                string destinationColumnReference = columnMappingElement.Attribute("destinationColumnReference").Value;

                                DBColumn destinationColumn = tableMapping.DestinationTable.Columns
                                    .Where(c => c.Name.ToLower() == destinationColumnReference.ToLower()).First();

                                ColumnMapping columnMapping = null;

                                if (type == typeof(ExcelColumnMapping).Name)
                                {
                                    string sourceHeader = columnMappingElement.Attribute("sourceHeader").Value;
                                    columnMapping = new ExcelColumnMapping(sourceHeader, destinationColumn, columnUse);
                                }
                                else if (type == typeof(TableColumnMapping).Name)
                                {

                                    int sourceTableMappingIndex = int.Parse(columnMappingElement.Attribute("sourceTableMappingIndex").Value);
                                    string sourceColumnReference = columnMappingElement.Attribute("sourceColumnReference").Value;

                                    TableMapping sourceTableMapping = tableMappings[sourceTableMappingIndex];
                                    DBColumn sourceColumn = sourceTableMapping.DestinationTable.Columns
                                        .Where(c => c.Name.ToLower() == sourceColumnReference.ToLower()).First();

                                    columnMapping = new TableColumnMapping(sourceTableMapping, sourceColumn, destinationColumn, columnUse);

                                }
                                else if (type == typeof(LiteralColumnMapping).Name)
                                {
                                    string litearal = columnMappingElement.Attribute("literal").Value;
                                    string literalType = columnMappingElement.Attribute("literalType").Value;

                                    columnMapping = new LiteralColumnMapping(litearal, 
                                        (LiteralType)Enum.Parse(typeof(LiteralType), literalType), destinationColumn, columnUse);
                                }
                                else
                                {
                                    columnMapping = new NullColumnMapping(destinationColumn, columnUse);
                                }

                                columnMappings.Add(columnMapping);
                            }


                        }

                        tableMapping.ColumnMappings = columnMappings.ToArray();
                    }

                }




            }


            return tableMappings.Values.ToArray();
        }


        private string readDatabaseName(XmlReader reader)
        {
            string databaseName = "";
            if (reader.ReadToFollowing("DatabaseName"))
            {
                databaseName = reader.ReadElementContentAsString();

            }
            return databaseName;
        }

        private ConnectionSetup readConnectionSetup(XmlReader reader)
        {
            ConnectionSetup connnectionSetup = null;

            if (reader.ReadToFollowing("ConnectionSetup"))
            {
                string serverName = reader.GetAttribute("serverName");
                bool windowsAuthentication = bool.Parse(reader.GetAttribute("windowsAuthentication"));
                string userName = reader.GetAttribute("userName");
                string password = reader.GetAttribute("password");
                int timeout = int.Parse(reader.GetAttribute("timeout"));

                connnectionSetup = new ConnectionSetup(serverName, userName, password, windowsAuthentication, timeout);
            }

            return connnectionSetup;
        }


    }
}
