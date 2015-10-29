/*
 * 
 * Presentation logic for the WizardMappingPage
 * 
 */



using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLImporter.ViewModel
{

    // ImportType holds the types of supported imports
    public enum ColumnMappingImportType { Excel, Null, Table, Literal, Function }

    public class MappingPageViewModel : Notifier
    {

        private readonly ColumnMappingImportType[] importTypeList =
            new ColumnMappingImportType[] { ColumnMappingImportType.Null, ColumnMappingImportType.Excel, ColumnMappingImportType.Table, ColumnMappingImportType.Literal };

        private readonly TableMappingImportType[] tableMappingImportTypeList =
             new TableMappingImportType[] { TableMappingImportType.Insert, TableMappingImportType.Update };


        private ObservableCollection<TableMappingViewModel> tableMappingViewModels = new ObservableCollection<TableMappingViewModel>();

        private WizardViewModel wizardViewModel;

        public MappingPageViewModel(WizardViewModel wizardViewModel)
        {
            this.wizardViewModel = wizardViewModel;
        }

        public WizardViewModel WizardViewModel
        {
            get { return wizardViewModel; }
        }

        public ColumnMappingImportType[] ImportTypes
        {
            get { return importTypeList; }
        }

        public LiteralType[] LiteralTypes
        {
            get { return new LiteralType[] { LiteralType.String, LiteralType.Integer, LiteralType.Function }; }
        }

        public TableMappingImportType[] TableMappingImportTypes
        {
            get { return tableMappingImportTypeList; }
        }


        public ObservableCollection<TableMappingViewModel> TableMappingViewModels
        {
            get
            {
                return tableMappingViewModels;
            }
        }

        public List<TableMapping> TableMappings
        {
            get { return tableMappingViewModels.Select(t => t.TableMapping).ToList(); }

            set
            {
                List<TableMapping> tableMappings = value;
                tableMappingViewModels.Clear();

                foreach (TableMapping tableMapping in tableMappings)
                {
                    TableMappingViewModel tableMappingViewModel = new TableMappingViewModel(tableMapping, this);
                    tableMappingViewModels.Add(tableMappingViewModel);
                }
                NotifyPropertyChanged("TableMappings");
                NotifyPropertyChanged("MappingTableReferences");
                NotifyPropertyChanged("TableMappingViewModels");
            }
        }

        public void AddMappingTable(DBTable table)
        {
            tableMappingViewModels.Add(new TableMappingViewModel(table, this));
            NotifyPropertyChanged("TableMappings");
            NotifyPropertyChanged("MappingTableReferences");
        }

        public void RemoveMappingTable(TableMappingViewModel mappingTable)
        {
            tableMappingViewModels.Remove(mappingTable);
            NotifyPropertyChanged("TableMappings");
            NotifyPropertyChanged("MappingTableReferences");
        }


        internal void tableMappingSwitchDatabase(TableMapping[] tableMappings, Database db)
        {

            Dictionary<int, TableMapping> newTableMappings = new Dictionary<int, TableMapping>();

            foreach (TableMapping tableMapping in tableMappings)
            {
                DBTable table = db.Tables.Where(t => t.Reference == tableMapping.DestinationTable.Reference).FirstOrDefault();
                if (table != null)
                {
                    TableMapping newTableMapping = new TableMapping(table, tableMapping.ImportType, new ColumnMapping[0]);
                    newTableMappings.Add(tableMapping.Index, newTableMapping);
                }
            }

            foreach (int index in newTableMappings.Keys)
            {
                TableMapping newTableMapping = newTableMappings[index];

                List<ColumnMapping> newColumnMappings = new List<ColumnMapping>();

                TableMapping oldTableMapping = tableMappings
                    .Where(t => t.Index == index).First();


                ColumnMapping[] oldColumnMappings = oldTableMapping.ColumnMappings;

                foreach (ColumnMapping oldColumnMapping in oldColumnMappings)
                {
                    DBColumn column = newTableMapping.DestinationTable.Columns
                        .Where(c => c.Name == oldColumnMapping.DestinationColumn.Name)
                        .FirstOrDefault();

                    if (column != null)
                    {
                        string type = oldColumnMapping.GetType().ToString();
                        ColumnMapping newColumnMapping = null;

                        if (type == typeof(ExcelColumnMapping).ToString())
                        {
                            var excelColumnMapping = (ExcelColumnMapping)oldColumnMapping;
                            newColumnMapping = new ExcelColumnMapping(excelColumnMapping.SourceHeader, column, oldColumnMapping.ColumnUse);
                        }
                        else if (type == typeof(TableColumnMapping).ToString())
                        {
                            var tableColumnMapping = (TableColumnMapping)oldColumnMapping;

                            if (newTableMappings.ContainsKey(tableColumnMapping.SourceTableMapping.Index))
                            {
                                TableMapping newSourceTableMapping = newTableMappings[tableColumnMapping.SourceTableMapping.Index];
                                DBColumn newSourceColumn = newSourceTableMapping.DestinationTable.Columns
                                    .Where(c => c.Name == tableColumnMapping.SourceColumn.Name)
                                    .FirstOrDefault();

                                if (newSourceColumn != null)
                                {
                                    newColumnMapping = new TableColumnMapping(newSourceTableMapping, newSourceColumn, column, oldColumnMapping.ColumnUse);
                                }
                            }
                        }
                        else if (type == typeof(LiteralColumnMapping).ToString())
                        {
                            var literalColumnMapping = (LiteralColumnMapping)oldColumnMapping;
                            newColumnMapping = new LiteralColumnMapping(literalColumnMapping.Literal, literalColumnMapping.LiteralType, column, literalColumnMapping.ColumnUse);
                        }

                        if (newColumnMapping == null)
                        {
                            newColumnMapping = new NullColumnMapping(column, oldColumnMapping.ColumnUse);
                        }

                        newColumnMappings.Add(newColumnMapping);
                    }
                }

                newTableMapping.ColumnMappings = newColumnMappings.ToArray();
            }

            this.TableMappings = newTableMappings.Values.ToList();
        }








    }
}
