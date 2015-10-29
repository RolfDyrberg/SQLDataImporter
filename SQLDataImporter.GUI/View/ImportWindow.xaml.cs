using SQLImporter.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {

        public ImportWindow(ImportViewModel importViewModel)
        {
            InitializeComponent();

            this.DataContext = importViewModel;

            ContentRendered += ImportWindow_ContentRendered;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var importViewModel = (ImportViewModel)DataContext;

            if (importViewModel.IsImporting)
            {
                base.OnClosing(e);
                e.Cancel = true;
            }
        }

        void ImportWindow_ContentRendered(object sender, EventArgs e)
        {
            var importViewModel = (ImportViewModel)this.DataContext;
            importViewModel.BackgroundImport();
        }


    }
}
