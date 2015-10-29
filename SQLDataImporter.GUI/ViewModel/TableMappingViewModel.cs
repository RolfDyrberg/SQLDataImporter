/*
 * 
 * Presentation logic for a TableMappingControl/Tablemapping
 * 
 */


using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLImporter.ViewModel
{
    
    public class TableMappingViewModel : Notifier
    {
        private MappingPageViewModel viewModel;

        private TableMapping tableMapping;

        private List<ColumnMappingViewModel> columnsMappingViewModels;

        public TableMappingViewModel(DBTable table, MappingPageViewModel mappingPageViewModel)
        {
            this.viewModel = mappingPageViewModel;

            ColumnMapping[] mappings = new ColumnMapping[table.Columns.Count];
            ObservableCollection<ColumnMappingViewModel> viewModels = new ObservableCollection<ColumnMappingViewModel>();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                mappings[i] = new NullColumnMapping(table.Columns[i], ColumnUse.Insert);
                viewModels.Add(new NullColumnMappingViewModel((NullColumnMapping)mappings[i], this));
            }

            this.tableMapping = new TableMapping(table, TableMappingImportType.Insert, mappings);
            this.columnsMappingViewModels = viewModels.ToList();
        }


        public TableMappingViewModel(TableMapping tableMapping, MappingPageViewModel mappingPageViewModel)
        {
            this.viewModel = mappingPageViewModel;

            ObservableCollection<ColumnMappingViewModel> viewModels = new ObservableCollection<ColumnMappingViewModel>();

            foreach (DBColumn column in tableMapping.DestinationTable.Columns)
            {
                ColumnMapping columnMapping = tableMapping.ColumnMappings.Where(c => c.DestinationColumn == column).FirstOrDefault();
                if (columnMapping == null) columnMapping = new NullColumnMapping(column, tableMapping.AllowedColumnUses()[0]);
                viewModels.Add(ColumnMappingViewModelCreator.CreateFromColumnMapping(columnMapping, this));
            }

            ColumnMapping[] columnMappings = viewModels.Select(v => v.ColumnMapping).ToArray();
            tableMapping.ColumnMappings = columnMappings;

            this.tableMapping = tableMapping;
            this.columnsMappingViewModels = viewModels.ToList();
        }


        public string Reference
        {
            get { return tableMapping.DestinationTable.Reference; }
        }

        public DBTable Table
        {
            get { return tableMapping.DestinationTable; }
        }

        public List<ColumnMappingViewModel> ColumnMappings
        {
            get { return columnsMappingViewModels
                .Where(c => c.ColumnMapping.ColumnUse != ColumnUse.Exclude).ToList(); }
            set { columnsMappingViewModels = value; }
        }

        public TableMapping TableMapping
        {
            get { return tableMapping; }
        }

        public TableMappingImportType TableMappingImportType
        {
            get { return tableMapping.ImportType; }
            set
            { 
                tableMapping.ImportType = value;

                foreach (ColumnMappingViewModel columnMappingViewModel in columnsMappingViewModels)
                {
                    columnMappingViewModel.ChangeAllowedColumnUses();
                }
            }
        }

        public MappingPageViewModel MappingPageViewModel
        {
            get { return viewModel; }
        }

        public void ReplaceColumnMappingViewModel(ColumnMappingViewModel existing, ColumnMappingViewModel replacement)
        {
            int oldViewModelIndex = columnsMappingViewModels.IndexOf(existing);
            int oldModelIndex = Array.IndexOf(tableMapping.ColumnMappings, existing.ColumnMapping);

            if (oldViewModelIndex != -1 && oldModelIndex != -1)
            {
                tableMapping.ColumnMappings[oldModelIndex] = replacement.ColumnMapping;
                columnsMappingViewModels.RemoveAt(oldViewModelIndex);
                columnsMappingViewModels.Insert(oldViewModelIndex, replacement);
            }
        }

        internal void UpdateColumnMappings()
        {
            NotifyPropertyChanged("ColumnMappings");
        }
    }


}
