using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SQLImporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WizardWindow : Window
    {

        private WizardViewModel viewModel;

        public WizardWindow()
        {
            InitializeComponent();
            viewModel = new WizardViewModel();
            this.DataContext = viewModel;

            ContentRendered += WizardWindow_ContentRendered;
        }

        void WizardWindow_ContentRendered(object sender, EventArgs e)
        {
            viewModel.WizardWindow = this;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            var tabControl = (TabControl)FindName("wizardTabControl");
            tabControl.SelectedIndex++;
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            var tabControl = (TabControl)FindName("wizardTabControl");
            if (tabControl.SelectedIndex > 0) tabControl.SelectedIndex--;
        }

        private void New_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Configuration file";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Configuration (.xml)|*.xml";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                viewModel.LoadConfiguration(dlg.FileName);
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.ConfigurationPath == null || viewModel.ConfigurationPath == "")
            {
                saveAsFileDialog();
            }
            else
            {
                viewModel.SaveConfiguration();
            }
        }

        private void Save_As_Button_Click(object sender, RoutedEventArgs e)
        {
            saveAsFileDialog();
        }

        private void saveAsFileDialog()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Configuration";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Configuration (.xml)|*.xml";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                viewModel.ConfigurationPath = dlg.FileName;
                viewModel.SaveConfiguration();
            }
        }


    }
}
