/*
 * 
 * Presentation logic for a WizardConnectionPage
 * 
 */



using SQLDataImporter.DatabaseConnector;
using SQLDataImporter.DatabaseModel;
using SQLDataImporter.DataReader;
using SQLImporter.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SQLImporter.ViewModel
{
    public class ConnectionPageViewModel : Notifier
    {

        private ExcelReader reader;
        private string[] sourceColumnsHeaders;
        private string sourcePath;

        private IDatabaseConnector dbConnector;
        private ConnectionSetup connectionSetup;
        private Database selectedDatabase;
        private List<string> databaseNames;

        private WizardViewModel wizardViewModel;


        public ConnectionPageViewModel(WizardViewModel wizardViewModel)
        {
            this.wizardViewModel = wizardViewModel;
            selectedDatabase = new Database("", new List<DBTable>());
        }

        public ExcelReader ExcelReader
        {
            get { return reader; }
        }

        public string SourcePath
        {
            get { return sourcePath; }
            set
            {
                try
                {
                    sourcePath = value;
                    readSource(sourcePath);
                    NotifyPropertyChanged("SourcePath");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void readSource(string sourcePath)
        {
            reader = new ExcelReader(sourcePath);
            readWorkSheet();

            SelectedWorkSheet = reader.WorkSheetNames[0];
            NotifyPropertyChanged("SourceWorkSheets");
        }

        private void readWorkSheet()
        {
            SourceColumnHeaders = reader.GetHeaderNames();
        }

        public string SelectedWorkSheet
        {
            get
            {
                if (reader != null)
                {
                    return reader.SelectedWorksheetName;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                try
                {
                    if (reader != null && reader.SelectedWorksheetName != value)
                    {
                        reader.SelectedWorksheetName = value;
                        readWorkSheet();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    reader.SelectedWorksheetName = "";
                    SourceColumnHeaders = new string[0];
                }
                NotifyPropertyChanged("SelectedWorkSheet");
            }
        }

        public string[] SourceWorkSheets
        {
            get
            {
                if (reader != null)
                {
                    return reader.WorkSheetNames;
                }
                else
                {
                    return new string[0];
                }
            }
        }

        public string[] SourceColumnHeaders
        {
            get { return sourceColumnsHeaders; }
            set
            {
                sourceColumnsHeaders = value;
                NotifyPropertyChanged("SourceColumnHeaders");
            }
        }


        public ConnectionSetup ConnectionSetup
        {
            get { return connectionSetup; }
            set
            {
                try
                {
                    dbConnector = new SQLServerConnector(value);
                    connectionSetup = value;
                    setDatabases(dbConnector);
                    NotifyPropertyChanged("ConnectionSetup");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public IDatabaseConnector DatabaseConnector
        {
            get { return dbConnector; }
        }


        private void setDatabases(IDatabaseConnector dbConnector)
        {
            if (dbConnector != null)
            {
                try
                {
                    databaseNames = dbConnector.GetDatabaseNames();
                }
                catch (SqlException e)
                {
                    databaseNames = new List<string>();
                    MessageBox.Show(e.Message, "SqlException", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                databaseNames = new List<string>();
            }

            if (databaseNames.Count > 0 && dbConnector.GetDatabaseNames().Contains(selectedDatabase.Name))
            {
                SelectedDatabase = dbConnector.GetDatabase(selectedDatabase.Name);
            }
            else if (databaseNames.Count > 0)
            {
                SelectedDatabase = dbConnector.GetDatabase(databaseNames[0]);
            }
            else
            {
                SelectedDatabase = null;
            }

            NotifyPropertyChanged("DatabaseNames");
        }


        public List<string> DatabaseNames
        {
            get { return databaseNames; }
        }

        public string SelectedDatabaseName
        {
            get { return selectedDatabase.Name; }
            set
            {
                DBLoadBackgroundWorker w = new DBLoadBackgroundWorker(value, dbConnector, this);
                LoadingWindow window = new LoadingWindow(w, "Connecting to Database...");
                window.Owner = wizardViewModel.WizardWindow;
                window.ShowDialog();
            }
        }

        public Database SelectedDatabase
        {
            get { return selectedDatabase; }
            set
            {
                selectedDatabase = value;
                NotifyPropertyChanged("SelectedDatabaseName");
                wizardViewModel.ChangeDatabase();
            }
        }

        public List<DBTable> SelectedDatabaseTables
        {
            get { return selectedDatabase.Tables; }
        }

    }


    
}
