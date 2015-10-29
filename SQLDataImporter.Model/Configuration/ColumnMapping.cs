/*
 *
 * ColumnMapping is a mapping of some data to the column of a database table
 * Each columnmapping has a ColumnUse describing how the column is used in the sql statement.
 * 
 * The different types of column mappings:
 * NullColumnMapping maps NULL value to the column
 * ExcelColumnMapping maps the column of an excel file to the column
 * TableColumnMapping maps a column in another table to the column
 * LiteralColumnMapping maps a literal value to the column this can be different types: string, integer and function,
 * where functions are SQL Server functions
 * 
 */

using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SQLDataImporter.Configuration
{

    public enum ColumnUse { Insert, Exclude, Where, Set };

    public abstract class ColumnMapping
    {
        private DBColumn destinationColumn;
        private ColumnUse columnUse;

        protected ColumnMapping(DBColumn destinationColumn, ColumnUse columnUse)
        {
            this.destinationColumn = destinationColumn;
            this.columnUse = columnUse;
        }

        public DBColumn DestinationColumn
        {
            get { return destinationColumn; }
            set { destinationColumn = value; }
        }

        public string DestinationColumnReference
        {
            get { return destinationColumn.Name; }
        }

        public ColumnUse ColumnUse
        {
            get { return columnUse; }
            set { columnUse = value; }
        }
    }

    public class NullColumnMapping : ColumnMapping
    {
        public NullColumnMapping(DBColumn destinationColumn, ColumnUse columnUse)
            : base(destinationColumn, columnUse) { }
    }

    public class ExcelColumnMapping : ColumnMapping
    {

        private string sourceHeaderName;

        public ExcelColumnMapping(string sourceHeaderName, DBColumn destinationColumn, ColumnUse columnUse)
            : base(destinationColumn, columnUse)
        {
            this.sourceHeaderName = sourceHeaderName;   
        }

        public string SourceHeader
        {
            get { return sourceHeaderName; }
            set { this.sourceHeaderName = value; }
        }
    }

    public class TableColumnMapping : ColumnMapping
    {
        private TableMapping sourceTableMapping;
        private DBColumn sourceColumn;

        public TableColumnMapping(TableMapping sourceTableMapping, DBColumn sourceColumn,
            DBColumn destinationColumn, ColumnUse columnUse)
            : base(destinationColumn, columnUse)
        {
            this.sourceTableMapping = sourceTableMapping;
            this.sourceColumn = sourceColumn;
        }

        public TableMapping SourceTableMapping
        {
            get { return sourceTableMapping; }
            set { sourceTableMapping = value; }
        }

        public DBColumn SourceColumn
        {
            get { return sourceColumn; }
            set { sourceColumn = value; }
        }

    }


    public enum LiteralType { String, Integer, Function };

    public class LiteralColumnMapping : ColumnMapping
    {

        private string literal;
        private LiteralType literalType;

        public LiteralColumnMapping(string literal, LiteralType literalType, DBColumn destinationColumn, ColumnUse columnUse)
            : base(destinationColumn, columnUse)
        {
            this.literal = literal;
            this.literalType = literalType;
        }

        public string Literal
        {
            get { return literal; }
            set { this.literal = value; }
        }

        public LiteralType LiteralType
        {
            get { return literalType; }
            set { literalType = value; }
        }
    }

}
