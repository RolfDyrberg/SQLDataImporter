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
    /// Interaction logic for ImportUserControl.xaml
    /// </summary>
    public partial class WizardImportPage : UserControl
    {

        public WizardImportPage()
        {
            InitializeComponent();
            this.Loaded += WizardImportPage_Loaded;
        }

        void WizardImportPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext.GetType() != typeof(ImportPageViewModel))
            {
                DataContext = ((WizardViewModel)this.DataContext).ImportPageViewModel;
            }
        }

        private ImportPageViewModel viewModel { get { return (ImportPageViewModel)DataContext; } }

        private void Preview_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql = viewModel.PreviewSQL();

                var previewWindow = new PreviewWindow(sql);
                previewWindow.Owner = Window.GetWindow(this);
                previewWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Import_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ImportViewModel importViewModel = viewModel.CreateImportViewModel();

                var importWindow = new ImportWindow(importViewModel);
                importWindow.Owner = Window.GetWindow(this);
                importWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

    }

}
