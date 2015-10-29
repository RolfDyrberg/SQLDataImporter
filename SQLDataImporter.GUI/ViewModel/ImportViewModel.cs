/*
  * 
  * Presentation logic for the ImportWindow
  * 
  */

using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseModel;
using SQLDataImporter.DataImporter;
using SQLDataImporter.DataReader;
using SQLDataImporter.StatementCreator;
using SQLImporter.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SQLImporter.ViewModel
{
    public class ImportViewModel : Notifier
    {

        private ImportConfiguration config;
        private IDataReader reader;
        private Database database;

        private BackgroundWorker bw = new BackgroundWorker();

        private List<string> importStates = new List<string>();
        private List<ImportResult> importResults = new List<ImportResult>();

        private UnsuccesfulImport[] unsuccesfulImports = new UnsuccesfulImport[0];


        private int importRowCount = 1;
        private int importProgress = 0;
        private bool isImporting = false;


        public ImportViewModel(ImportConfiguration config, IDataReader reader, Database database)
        {
            this.config = config;
            this.reader = reader;
            this.database = database;

            bw.WorkerSupportsCancellation = false;
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_DoWorkCompleted);
        }

        public string[] ImportStates
        {
            get { return importStates.ToArray(); }
        }

        public UnsuccesfulImport[] UnsuccesfulImports
        {
            get { return unsuccesfulImports; }
        }

        public int ImportProgress
        {
            get { return importProgress; }
        }

        public int ImportRowCount
        {
            get { return importRowCount; }
        }

        public bool IsImporting
        {
            get { return isImporting; }
        }


        public void BackgroundImport()
        {
            bw.RunWorkerAsync();
        }

        public void bw_DoWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                importStates.Add("Import aborted");
                NotifyPropertyChanged("ImportStates");
                finishImport();
                return;
            }
        }

        public void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            isImporting = true;

            importStates.Add("Starting import...");
            NotifyPropertyChanged("ImportStates");

            importStates.Add("Reading excel file");
            NotifyPropertyChanged("ImportStates");

            SourceDataTable dataTable = reader.ReadToDataTable();
            importRowCount = dataTable.NumberOfRows;
            NotifyPropertyChanged("ImportRowCount");

            importStates.Add("Creating SQL statements");
            NotifyPropertyChanged("ImportStates");

            BackgroundWorker worker = sender as BackgroundWorker;
            

            SQLServerStatementCreator statementCreator = new SQLServerStatementCreator(config, dataTable);

            ImportStatement[] statements = statementCreator.CreateStatements();

            importStates.Add("Importing data...");
            NotifyPropertyChanged("ImportStates");

            SQLServerDataImporter dataImporter = new SQLServerDataImporter(config);

            foreach (ImportStatement statement in statements)
            {
                ImportResult result = dataImporter.ImportData(statement);
                importResults.Add(result);

                importProgress++;
                NotifyPropertyChanged("ImportProgress");

                importStates[3] = String.Format("Importing data... row {0} of {1}", importProgress, statements.Length);
                NotifyPropertyChanged("ImportStates");
            }
            importStates.Add("Import complete");
            NotifyPropertyChanged("ImportStates");

            finishImport();
        }

        private void finishImport()
        {
            SuccesfulImport[] succesfulImports = importResults.Where(r => r.GetType() == typeof(SuccesfulImport)).Select(s => (SuccesfulImport)s).ToArray();
            updateUnsuccesfulImports();

            string excelRowsImported = String.Format("Excel rows succesfully imported: {0}", succesfulImports.Count());
            string dbRowAffected = String.Format("Database rows affected: {0}", succesfulImports.Sum(r => r.RowsAffected));
            string rowsWithErrors = String.Format("Excel rows with error in import: {0}", unsuccesfulImports.Count());

            importStates.Add(excelRowsImported);
            importStates.Add(dbRowAffected);
            importStates.Add(rowsWithErrors);
            NotifyPropertyChanged("ImportStates");


            isImporting = false;
        
        }

        private void updateUnsuccesfulImports()
        {
                    unsuccesfulImports = importResults.Where(r => r.GetType() == typeof(UnsuccesfulImport)).Select(s => (UnsuccesfulImport)s).ToArray();
                    NotifyPropertyChanged("UnsuccesfulImports");
        }

  
    }
}
