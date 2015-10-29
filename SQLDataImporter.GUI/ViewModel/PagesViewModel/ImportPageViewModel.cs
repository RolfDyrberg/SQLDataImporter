/*
 * 
 * Preesentation logic for a WizardImportPage
 * 
 */



using SQLDataImporter.Configuration;
using SQLDataImporter.DataReader;
using SQLDataImporter.StatementCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLImporter.ViewModel
{
    public class ImportPageViewModel : Notifier
    {

        private ErrorHandling errorHandling = new ErrorHandling();

        private WizardViewModel wizardViewModel;

        public ImportPageViewModel(WizardViewModel wizardViewModel)
        {
            this.wizardViewModel = wizardViewModel;
        }

        public WizardViewModel WizardViewModel
        {
            get { return wizardViewModel; }
        }

        public ErrorHandling ErrorHandling
        {
            get { return errorHandling; }
            set
            {
                errorHandling = value;
                NotifyPropertyChanged("ErrorHandling");
            }
        }


        private ImportConfiguration createImportConfiguration()
        {
            if (wizardViewModel.ConnectionPageViewModel.DatabaseConnector == null)
            {
                throw new Exception("Not connected to any database");
            }
            else if (wizardViewModel.MappingPageViewModel.TableMappings.Count < 1)
            {
                throw new Exception("There are no table mappings");
            }
            else
            {
                TableMapping[] tableMappings = wizardViewModel.MappingPageViewModel.TableMappingViewModels.Select(t => t.TableMapping).ToArray();
                ImportConfiguration config = new ImportConfiguration(tableMappings, wizardViewModel.ConnectionPageViewModel.DatabaseConnector.ConnectionSetup,
                    wizardViewModel.ConnectionPageViewModel.SelectedDatabase.Name, errorHandling);

                return config;
            }
        }

        public string PreviewSQL()
        {
            if (wizardViewModel.ConnectionPageViewModel.ExcelReader == null)
            {
                throw new Exception("No excel file selected.");
            }
            else
            {
                SourceDataTable dataTable = wizardViewModel.ConnectionPageViewModel.ExcelReader.ReadFirstRowToDataTable();

                SQLServerStatementCreator statementCreator = new SQLServerStatementCreator(createImportConfiguration(), dataTable);
                ImportStatement previewStatement = statementCreator.CreateStatement(0);
                return previewStatement.SqlStatement;
            }
        }


        public ImportViewModel CreateImportViewModel()
        {
            if (wizardViewModel.ConnectionPageViewModel.ExcelReader == null)
            {
                throw new Exception("No excel file selected.");
            }
            else
            {
                return new ImportViewModel(createImportConfiguration(), wizardViewModel.ConnectionPageViewModel.ExcelReader,
                    wizardViewModel.ConnectionPageViewModel.SelectedDatabase);
            }
        }



    }
}
