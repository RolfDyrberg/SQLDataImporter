/*
 * 
 * ConfigurationSaver saves an ImportConfiguration to a file in an xml format.
 * 
 * 
 */


using SQLDataImporter.DatabaseConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SQLDataImporter.Configuration
{
    public class ConfigurationSaver
    {

        private ImportConfiguration configuration;
        private string filePath;

        public ConfigurationSaver(ImportConfiguration configuration, string filePath)
        {
            this.configuration = configuration;
            this.filePath = filePath;
        }

        public void Save()
        {
            using (XmlWriter writer = XmlWriter.Create(@filePath))
            {
                writer.WriteStartDocument();
                writeImportConfiguration(writer);

                writer.WriteEndDocument();
            }
        }

        private void writeImportConfiguration(XmlWriter writer)
        {
            writer.WriteStartElement("ImportConfiguration");

            writer.WriteRaw("\n");

            writeConnectionSetup(writer);
            writer.WriteRaw("\n");

            writeDatabaseName(writer);
            writer.WriteRaw("\n");

            writeErrorHandling(writer);
            writer.WriteRaw("\n");

            writeTableMappings(writer);

            writer.WriteRaw("\n");
            writer.WriteEndElement();
        }

        private void writeErrorHandling(XmlWriter writer)
        {
            writer.WriteStartElement("ErrorHandling");

            writer.WriteStartAttribute("ignoreWarnings");
            writer.WriteValue(configuration.ErrorHandling.IgnoreWarnings);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("outputRowNumbers");
            writer.WriteValue(configuration.ErrorHandling.OutputRowNumbers);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("importAsTransaction");
            writer.WriteValue(configuration.ErrorHandling.OutputRowNumbers);
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

        private void writeTableMappings(XmlWriter writer)
        {
            writer.WriteStartElement("TableMappings");
            writer.WriteRaw("\n");

            foreach (TableMapping tableMapping in configuration.TableMappings)
            {
                writer.WriteStartElement("TableMapping");
                writer.WriteStartAttribute("index");
                writer.WriteValue(tableMapping.Index);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("destinationTableReference");
                writer.WriteValue(tableMapping.DestinationTable.Reference);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("importType");
                writer.WriteValue(tableMapping.ImportType.ToString());
                writer.WriteEndAttribute();

                writer.WriteRaw("\n");
                writer.WriteStartElement("ColumnMappings");
                writer.WriteRaw("\n");
                
                foreach (ColumnMapping columnMapping in tableMapping.ColumnMappings)
                {
                    writeColumnMapping(writer, columnMapping);
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteRaw("\n");
            }

            writer.WriteEndElement();
        }

        private static void writeColumnMapping(XmlWriter writer, ColumnMapping columnMapping)
        {
            writer.WriteStartElement("ColumnMapping");

            writer.WriteStartAttribute("type");
            writer.WriteValue(columnMapping.GetType().Name);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("columnUse");
            writer.WriteValue(columnMapping.ColumnUse.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("destinationColumnReference");
            writer.WriteValue(columnMapping.DestinationColumn.Name);
            writer.WriteEndAttribute();

            if (columnMapping is ExcelColumnMapping)
            {
                var excelColumnMapping = (ExcelColumnMapping)columnMapping;

                writer.WriteStartAttribute("sourceHeader");
                writer.WriteValue(excelColumnMapping.SourceHeader);
                writer.WriteEndAttribute();

            }
            else if (columnMapping is TableColumnMapping)
            {
                var tableColumnMapping = (TableColumnMapping)columnMapping;

                writer.WriteStartAttribute("sourceTableMappingIndex");
                writer.WriteValue(tableColumnMapping.SourceTableMapping.Index);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("sourceColumnReference");
                writer.WriteValue(tableColumnMapping.SourceColumn.Name);
                writer.WriteEndAttribute();
            }
            else if (columnMapping is LiteralColumnMapping)
            {
                var literalColumnMapping = (LiteralColumnMapping)columnMapping;

                writer.WriteStartAttribute("literal");
                writer.WriteValue(literalColumnMapping.Literal);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("literalType");
                writer.WriteValue(literalColumnMapping.LiteralType.ToString());
                writer.WriteEndAttribute();
            }

            writer.WriteEndElement();
            writer.WriteRaw("\n");
        }

        private void writeDatabaseName(XmlWriter writer)
        {
            writer.WriteStartElement("DatabaseName");
            writer.WriteValue(configuration.DatabaseName);
            writer.WriteEndElement();
        }

        private void writeConnectionSetup(XmlWriter writer)
        {
            writer.WriteStartElement("ConnectionSetup");

            ConnectionSetup connectionSetup = configuration.ConnectionSetup;

            writer.WriteStartAttribute("serverName");
            writer.WriteValue(connectionSetup.ServerName);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("windowsAuthentication");
            writer.WriteValue(connectionSetup.UseWindowsAuthentication);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("userName");
            writer.WriteValue(connectionSetup.UserName);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("password");
            writer.WriteValue(connectionSetup.Password);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("timeout");
            writer.WriteValue(connectionSetup.Timeout);
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

    }

}
