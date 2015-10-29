/*
 * 
 * Database is a representaion of a SQLServer database
 * It contains DBTables, which represent tables and DBColumns which represent columns.
 * 
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DatabaseModel
{

    public enum DBDatatype { integer, nvarchar, varchar, datetime };

    public class Database
    {
        private string name;
        private List<DBTable> tables;
        private List<DBForeignKey> foreignKeys;

        public Database(string name, List<DBTable> tables)
        {
            this.name = name;
            this.tables = tables;
            this.foreignKeys = new List<DBForeignKey>();
        }

        public string Name
        {
            get { return name; }
        }

        public List<DBTable> Tables
        {
            get { return new List<DBTable>(tables); }
            internal set { this.tables = value; }
        }

        public List<DBForeignKey> ForeignKeys
        {
            get { return new List<DBForeignKey>(foreignKeys); }
            internal set { this.foreignKeys = value; }
        }

        public DBColumn GetColumn(string schemaName, string tableName, string columnName)
        {
            DBTable table = tables.Where<DBTable>(t => t.Name == tableName && t.Schema == schemaName).FirstOrDefault();
            if (table != null) return table.Columns.Where(c => c.Name == columnName).FirstOrDefault();
            else return null;
        }

        public void AddForeignKeys(DBForeignKey foreignKey)
        {
            foreignKeys.Add(foreignKey);
        }
    }

    public class DBTable
    {
        private string schema;
        private string name;
        private List<DBColumn> columns;

        public DBTable(string schema, string name)
        {
            this.schema = schema;
            this.name = name;
            this.columns = new List<DBColumn>();
        }

        public DBTable(string schema, string name, List<DBColumn> columns)
        {
            this.schema = schema;
            this.name = name;
            this.columns = columns;
        }

        public string Schema
        {
            get { return schema; }
        }

        public string Name
        {
            get { return name; }
        }

        public List<DBColumn> Columns
        {
            get { return new List<DBColumn>(columns);  }
            set { this.columns = value; }
        }

        internal void addColumn(DBColumn column)
        {
            this.columns.Add(column);
        }

        public string Reference 
        {
            get { return Schema + "." + Name; }
        }
    }

    public class DBColumn
    {

        private DBTable table;

        private string name;
        private DBDatatype dataType;
        private bool isPrimaryKey;

        //TODO: Support more column metadata?
        //private bool isNullable;
        //private int charMaxLength;
        //private int numericPrecision;
        //private int datetimePrecision;

        //private DatabaseCharset charSet;


        public DBColumn(DBTable table, string name, bool isPrimaryKey, DBDatatype datatype)
        {
            this.table = table;
            this.name = name;
            this.isPrimaryKey = isPrimaryKey;
            this.dataType = datatype;
        }

        public string Name
        {
            get { return name; }
        }

        public bool IsPrimaryKey
        {
            get { return isPrimaryKey; }
        }

        public DBDatatype DataType
        {
            get { return dataType; }
        }

        public DBTable Table
        {
            get { return table; }
        }
    }



    public class DBForeignKey
    {
        private DBTable primaryKeyTable;
        private DBColumn primaryKeyCol;

        private DBTable foreignKeyTable;
        private DBColumn foreignKeyCol;

        public DBForeignKey(DBTable primaryKeyTable, DBColumn primaryKeyCol, DBTable foreignKeyTable, DBColumn foreignKeyCol)
        {
            this.primaryKeyTable = primaryKeyTable;
            this.primaryKeyCol = primaryKeyCol;
            this.foreignKeyTable = primaryKeyTable;
            this.foreignKeyCol = foreignKeyCol;
        }

        public DBTable PrimaryKeyTable
        {
            get { return primaryKeyTable; }
        }

        public DBColumn PrimaryKeyColumn
        {
            get { return primaryKeyCol; }
        }

        public DBTable ForeignKeyTable
        {
            get { return foreignKeyTable; }
        }

        public DBColumn ForeignKeyColumn
        {
            get { return foreignKeyCol; }
        }

    }



}
