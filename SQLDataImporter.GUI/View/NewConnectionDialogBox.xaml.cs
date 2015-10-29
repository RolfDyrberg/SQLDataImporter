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
    /// Interaction logic for NewConnectionDialogBox.xaml
    /// </summary>
    public partial class NewConnectionDialogBox : Window
    {
        public NewConnectionDialogBox()
        {
            InitializeComponent();
            defaultSetup();
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

        private void ok_button_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
