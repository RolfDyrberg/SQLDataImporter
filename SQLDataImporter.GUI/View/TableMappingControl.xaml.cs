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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SQLImporter.View
{
    /// <summary>
    /// Interaction logic for TableMappingControl.xaml
    /// </summary>
    public partial class TableMappingControl : UserControl
    {
        private MappingPageViewModel mappingPageViewModel;
        private TableMappingViewModel tableMappingViewModel;

        public TableMappingControl()
        {
            InitializeComponent();
            this.Loaded += TableMappingControl_Loaded;
        }

        void TableMappingControl_Loaded(object sender, RoutedEventArgs e)
        {
            tableMappingViewModel = (TableMappingViewModel)DataContext;
            mappingPageViewModel = ((WizardViewModel)Window.GetWindow(this).DataContext).MappingPageViewModel;
        }

        void removeButton_Click(object sender, RoutedEventArgs e)
        {
            mappingPageViewModel.RemoveMappingTable(tableMappingViewModel);
        }

        void columnsButton_Click(object sender, RoutedEventArgs e)
        {
            ColumnPicker columnPicker = new ColumnPicker(tableMappingViewModel);
            columnPicker.ShowDialog();
            tableMappingViewModel.UpdateColumnMappings();
        }




    }
}
