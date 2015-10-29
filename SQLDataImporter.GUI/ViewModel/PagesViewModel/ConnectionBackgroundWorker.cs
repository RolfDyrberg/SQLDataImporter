/*
 * 
 * Presentation logic connecting to a database.
 * Makes this connection run in its own thread and shows a loading screen while it runs.
 * 
 * 
 */


using SQLDataImporter.DatabaseConnector;
using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SQLImporter.ViewModel
{
    public class DBLoadBackgroundWorker : BackgroundWorker
    {
        private ConnectionPageViewModel viewModel;
        private string dbName;
        private IDatabaseConnector dbConnector;

        private Database db;

        public DBLoadBackgroundWorker(string dbName, IDatabaseConnector dbConnector, ConnectionPageViewModel viewModel)
            : base()
        {
            this.viewModel = viewModel;
            this.dbName = dbName;
            this.dbConnector = dbConnector;

            this.DoWork += DBBackgroundWorker_DoWork;
            this.RunWorkerCompleted += DBBackgroundWorker_RunWorkerCompleted;
        }

        void DBBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (db != null)
            {
                viewModel.SelectedDatabase = db;
            }
        }

        void DBBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                db = dbConnector.GetDatabase(dbName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
