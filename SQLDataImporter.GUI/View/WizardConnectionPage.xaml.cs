using Microsoft.Win32;
using SQLDataImporter.DatabaseConnector;
using SQLImporter.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for WizardConnectionPage.xaml
    /// </summary>
    public partial class WizardConnectionPage : UserControl
    {
        public WizardConnectionPage()
        {
            InitializeComponent();
            this.Loaded += WizardConnectionPage_Loaded;
            defaultSetup();

            
        }

        void WizardConnectionPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext.GetType() != typeof(ConnectionPageViewModel))
            {
                DataContext = ((WizardViewModel)this.DataContext).ConnectionPageViewModel;
            }
        }

        private ConnectionPageViewModel viewModel { get { return (ConnectionPageViewModel)DataContext; } }


        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            var serverNameTextBox = (TextBox)FindName("serverNameTextBox");
            var userNameTextBox = (TextBox)FindName("userNameTextBox");
            var passwordPasswordBox = (PasswordBox)FindName("passwordPasswordBox");
            var winauthRadiobutton = (RadioButton)FindName("winauthRadiobutton");

            string serverName = serverNameTextBox.Text;
            string userName = userNameTextBox.Text;
            string password = passwordPasswordBox.Password;
            bool useWinauth = false;

            if (winauthRadiobutton.IsChecked.HasValue
                    && winauthRadiobutton.IsChecked.Value)
            {
                useWinauth = true;
            }

            ConnectionSetup connSetup = new ConnectionSetup(serverName, userName, password, useWinauth);
            viewModel.ConnectionSetup = connSetup;            
        }


        private void Excel_Browse_File_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Excel file";
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel 2007 (.xlsx)|*.xlsx";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                viewModel.SourcePath = dlg.FileName;
            }
        }


        private void defaultSetup()
        {
            var winauthRadiobutton = (RadioButton)FindName("winauthRadiobutton");
            winauthRadiobutton.IsChecked = true;
        }

        private void windowsAuthSelect()
        {
            var userNameTextBox = (TextBox)FindName("userNameTextBox");
            var passwordPasswordBox = (PasswordBox)FindName("passwordPasswordBox");

            userNameTextBox.Background = Brushes.LightGray;
            passwordPasswordBox.Background = Brushes.LightGray;

            userNameTextBox.IsReadOnly = true;
            passwordPasswordBox.Focusable = false;
            passwordPasswordBox.PreviewTextInput += PasswordBoxReadOnly;
            passwordPasswordBox.PreviewKeyDown += PasswordBoxReadOnly;
            passwordPasswordBox.MouseRightButtonUp += PasswordBoxReadOnly;
        }

        private void PasswordBoxReadOnly(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void sqlAuthSelect()
        {
            var userNameTextBox = (TextBox)FindName("userNameTextBox");
            var passwordPasswordBox = (PasswordBox)FindName("passwordPasswordBox");

            userNameTextBox.Background = Brushes.White;
            passwordPasswordBox.Background = Brushes.White;

            userNameTextBox.IsReadOnly = false;
            passwordPasswordBox.Focusable = true;
            passwordPasswordBox.PreviewTextInput -= PasswordBoxReadOnly;
            passwordPasswordBox.PreviewKeyDown -= PasswordBoxReadOnly;
            passwordPasswordBox.MouseRightButtonUp -= PasswordBoxReadOnly;
        }

        private void winauth_checked(object sender, RoutedEventArgs e)
        {
            windowsAuthSelect();
        }

        private void sqlauth_checked(object sender, RoutedEventArgs e)
        {
            sqlAuthSelect();
        }

    }
}
