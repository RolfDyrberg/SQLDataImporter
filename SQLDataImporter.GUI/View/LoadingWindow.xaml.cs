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
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {

        private BackgroundWorker bw;

        public LoadingWindow(BackgroundWorker bw, string loadingLabel)
        {
            InitializeComponent();

            var loadingLabelControl = (Label)FindName("loadingLabel");
            loadingLabelControl.Content = loadingLabel;

            this.bw = bw;

            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.WorkerSupportsCancellation = false;
            bw.WorkerReportsProgress = false;

            ContentRendered += LoadingWindow_ContentRendered;
        }

       

        void LoadingWindow_ContentRendered(object sender, EventArgs e)
        {
            bw.RunWorkerAsync();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            this.Close();
        }

        




    }
}
