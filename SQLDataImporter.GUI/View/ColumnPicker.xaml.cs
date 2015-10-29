using SQLDataImporter.Configuration;
using SQLImporter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SQLImporter.View
{
    /// <summary>
    /// Interaction logic for ColumnPicker.xaml
    /// </summary>
    public partial class ColumnPicker : Window
    {

        private TableMappingViewModel viewModel;
        private ColumnPickerItem[] items = new ColumnPickerItem[0];

        public ColumnPicker(TableMappingViewModel tableMappingViewModel)
        {
            InitializeComponent();
            DataContext = this;

            this.viewModel = tableMappingViewModel;

            items = tableMappingViewModel.TableMapping.ColumnMappings.Select(c => new ColumnPickerItem(c)).ToArray();

            Closed += ColumnPicker_Closed;
        }

        void ColumnPicker_Closed(object sender, EventArgs e)
        {

            foreach (ColumnPickerItem item in items)
            {
                ColumnMapping columnMapping = item.ColumnMapping;
                if (item.IsSelected && columnMapping.ColumnUse == ColumnUse.Exclude)
                {
                    columnMapping.ColumnUse = viewModel.TableMapping.AllowedColumnUses()[0];
                }
                else if (!item.IsSelected)
                {
                    columnMapping.ColumnUse = ColumnUse.Exclude;
                }
            }
            viewModel.UpdateColumnMappings();
        }

        public ColumnPickerItem[] Items
        {
            get { return items; }
        }
    }


    public class ColumnPickerItem
    {

        private ColumnMapping columnMapping;
        private bool isSelected;

        public ColumnPickerItem(ColumnMapping columnMapping)
        {
            this.columnMapping = columnMapping;
            this.isSelected = columnMapping.ColumnUse != ColumnUse.Exclude;
        }

        public ColumnMapping ColumnMapping
        {
            get { return columnMapping; }
        }

        public string Name
        {
            get { return columnMapping.DestinationColumn.Name; }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }
    }

}
