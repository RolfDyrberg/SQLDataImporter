/*
 * 
 * Presentation logic for a ColumnMappingControl/ColumnMapping
 * 
 */



using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLImporter.ViewModel
{
    public abstract class ColumnMappingViewModel : Notifier
    {

        protected ColumnMappingImportType importType;
        protected TableMappingViewModel tableMappingViewModel;
        protected ColumnMapping columnMapping;

        public ColumnMappingViewModel(ColumnMapping columnMapping,
            TableMappingViewModel tableMappingViewModel)
        {
            this.tableMappingViewModel = tableMappingViewModel;
            this.columnMapping = columnMapping;

            
        }

        public ColumnMapping ColumnMapping
        {
            get { return columnMapping; }
        }

        public DBColumn Column
        {
            get { return columnMapping.DestinationColumn; }
        }

        public ColumnMappingImportType ImportType
        {
            get { return importType; }
            set
            {
               if (importType != value)
                {
                    tableMappingViewModel.ReplaceColumnMappingViewModel(this,
                        ColumnMappingViewModelCreator.CreateBlankSourceViewModel(value, tableMappingViewModel, columnMapping.DestinationColumn, columnMapping.ColumnUse));
                }
               NotifyPropertyChanged("ImportType");
               NotifyPropertyChanged("ColumnMapping");
               tableMappingViewModel.UpdateColumnMappings();
            }
        }

        public ColumnUse ColumnUse
        {
            get { return columnMapping.ColumnUse; }
            set 
            {
                if (columnMapping.ColumnUse != value)
                {
                    columnMapping.ColumnUse = value;
                    NotifyPropertyChanged("ColumnUse");
                    tableMappingViewModel.UpdateColumnMappings();
                }
            }
        }

        public void ChangeAllowedColumnUses()
        {
            NotifyPropertyChanged("AllowedColumnUses");
            ColumnUse = AllowedColumnUses[0];
        }

        public ColumnUse[] AllowedColumnUses
        {
            get { return tableMappingViewModel
                    .TableMapping
                    .AllowedColumnUses()
                    .Where(c => c != ColumnUse.Exclude).ToArray(); }
        }

        public string NameEscaped
        {
            get { return columnMapping.DestinationColumn.Name.Replace("_", "__"); }
        }

    }


    public class NullColumnMappingViewModel : ColumnMappingViewModel
    {

        public NullColumnMappingViewModel(NullColumnMapping nullColumnMapping, TableMappingViewModel tableMappingViewModel) :
            base(nullColumnMapping, tableMappingViewModel)
        {
            this.importType = ColumnMappingImportType.Null;
        }
    }

    public class ExcelColumnMappingViewModel : ColumnMappingViewModel
    {

        public ExcelColumnMappingViewModel(ExcelColumnMapping excelColumnMapping, TableMappingViewModel tableMappingViewModel) :
            base(excelColumnMapping, tableMappingViewModel)
        {
            this.importType = ColumnMappingImportType.Excel;
        }

        private ExcelColumnMapping excelColumnMapping
        {
            get { return (ExcelColumnMapping)columnMapping; }
        }

        public string SourceHeader
        {
            get 
            {
                return excelColumnMapping.SourceHeader; 
            }
            set
            {
                excelColumnMapping.SourceHeader = value;
                NotifyPropertyChanged("SourceHeader");
            }
        }
    }

    public class TableColumnMappingViewModel : ColumnMappingViewModel
    {

        public TableColumnMappingViewModel(TableColumnMapping tableColumnMapping,
            TableMappingViewModel tableMappingViewModel)
            : base(tableColumnMapping, tableMappingViewModel)
        {
            this.importType = ColumnMappingImportType.Table;
        }


        public TableMapping SourceTableMapping
        {
            get { return ((TableColumnMapping)columnMapping).SourceTableMapping; }
            set
            {
                TableMapping sourceTableMapping = ((TableColumnMapping)columnMapping).SourceTableMapping;
                if (sourceTableMapping != value)
                {
                    sourceTableMapping = value;
                    tableColumnMapping.SourceTableMapping = value;
                    NotifyPropertyChanged("SourceTableMapping");
                    NotifyPropertyChanged("SourceColumnMappings");
                }
            }
        }

        public DBColumn SourceColumn
        {
            get { return ((TableColumnMapping)columnMapping).SourceColumn; }
            set
            {
                DBColumn sourceColumn = ((TableColumnMapping)columnMapping).SourceColumn;
                sourceColumn = value;
                tableColumnMapping.SourceColumn = value;
                NotifyPropertyChanged("SourceColumnMapping");
            }
        }

        private TableColumnMapping tableColumnMapping
        {
            get { return (TableColumnMapping)columnMapping; }
        }

        public DBColumn[] SourceColumnMappings
        {
            get
            {
                TableMapping sourceTableMapping = ((TableColumnMapping)columnMapping).SourceTableMapping;
                if (sourceTableMapping != null) return sourceTableMapping.ColumnMappings.Select(c => c.DestinationColumn).ToArray();
                return new DBColumn[0];
            }
        }
    }

    public class LiteralColumnMappingViewModel : ColumnMappingViewModel
    {

        public LiteralColumnMappingViewModel(LiteralColumnMapping literalColumnMapping,
            TableMappingViewModel tableMappingViewModel)
            : base(literalColumnMapping, tableMappingViewModel)
        {
            this.importType = ColumnMappingImportType.Literal;
        }
        
        public LiteralColumnMapping literalColumnMapping
        {
            get { return (LiteralColumnMapping)columnMapping; }
        }

        public string Literal
        {
            get { return literalColumnMapping.Literal; }
            set
            {
                literalColumnMapping.Literal = value;
                NotifyPropertyChanged("Literal");
            }
        }

        public LiteralType LiteralType
        {
            get { return literalColumnMapping.LiteralType; }
            set
            { 
                literalColumnMapping.LiteralType = value;
                NotifyPropertyChanged("LiteralType");
            }
        }
    }


    public class ColumnMappingViewModelCreator
    {
        public static ColumnMappingViewModel CreateBlankSourceViewModel(ColumnMappingImportType importType, TableMappingViewModel tableMappingViewModel, 
            DBColumn destinationColumn, ColumnUse columnUse)
        {
            switch (importType)
            {                   
                case (SQLImporter.ViewModel.ColumnMappingImportType.Excel):
                    ExcelColumnMapping excelColumnMapping = new ExcelColumnMapping("", destinationColumn, columnUse);
                    return new ExcelColumnMappingViewModel(excelColumnMapping, tableMappingViewModel);
                case (SQLImporter.ViewModel.ColumnMappingImportType.Table):
                    TableColumnMapping tableColumnMapping = new TableColumnMapping(null, null, destinationColumn, columnUse);
                    return new TableColumnMappingViewModel(tableColumnMapping, tableMappingViewModel);
                case (SQLImporter.ViewModel.ColumnMappingImportType.Literal):
                    LiteralColumnMapping literalColumnMapping = new LiteralColumnMapping("", LiteralType.String, destinationColumn, columnUse);
                    return new LiteralColumnMappingViewModel(literalColumnMapping, tableMappingViewModel);
                default:
                    NullColumnMapping nullColumnMapping = new NullColumnMapping(destinationColumn, columnUse);
                    return new NullColumnMappingViewModel(nullColumnMapping, tableMappingViewModel);
            }
        }

        public static ColumnMappingViewModel CreateFromColumnMapping(ColumnMapping columnMapping, 
            TableMappingViewModel tableMappingViewModel)
        {

            string type = columnMapping.GetType().ToString();

            if (type == typeof(ExcelColumnMapping).ToString())
            {
                return new ExcelColumnMappingViewModel((ExcelColumnMapping)columnMapping, tableMappingViewModel);
            }
            else if (type == typeof(TableColumnMapping).ToString())
            {
                return new TableColumnMappingViewModel((TableColumnMapping)columnMapping, tableMappingViewModel);
            }
            else if (type == typeof(LiteralColumnMapping).ToString())
            {
                return new LiteralColumnMappingViewModel((LiteralColumnMapping)columnMapping, tableMappingViewModel);
            }
            else
            {
                return new NullColumnMappingViewModel((NullColumnMapping)columnMapping, tableMappingViewModel);
            }
        }
    }
}
