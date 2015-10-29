using SQLDataImporter.DatabaseModel;
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
    /// Interaction logic for WizardMappingPage.xaml
    /// </summary>
    public partial class WizardMappingPage : UserControl
    {

        public WizardMappingPage()
        {
            InitializeComponent();
            this.Loaded += WizardMappingPage_Loaded;
        }

        void WizardMappingPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext.GetType() != typeof(MappingPageViewModel))
            {
                DataContext = ((WizardViewModel)this.DataContext).MappingPageViewModel;
            }
        }

        private MappingPageViewModel viewModel { get { return (MappingPageViewModel)DataContext; } }

        private void Add_Table_Button_Click(object sender, RoutedEventArgs e)
        {
            AddTableToMappingDialog dlg = new AddTableToMappingDialog();
            dlg.DataContext = viewModel.WizardViewModel;
            dlg.Owner = Window.GetWindow(this);
            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                var tablesListBox = (ListBox)dlg.FindName("TablesListBox");
                DBTable selectedTable = (DBTable)tablesListBox.SelectedItem;
                if (selectedTable != null) viewModel.AddMappingTable(selectedTable);
            }

        }
    }
}
