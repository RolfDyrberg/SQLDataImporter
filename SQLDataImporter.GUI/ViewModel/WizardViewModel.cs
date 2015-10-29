/*
 * 
 * Presentation logic for the WizardWindow
 * 
 */


using SQLDataImporter.DataImporter;
using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseConnector;
using SQLDataImporter.DatabaseModel;
using SQLDataImporter.DataReader;
using SQLDataImporter.StatementCreator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SQLImporter.View;

namespace SQLImporter.ViewModel
{

    public class WizardViewModel : Notifier
    {

        private string configurationPath;

        private WizardWindow wizardWindow;

        private ConnectionPageViewModel connectionPageViewModel;
        private MappingPageViewModel mappingPageViewModel;
        private ImportPageViewModel importPageViewModel;


        public WizardViewModel()
        {
            connectionPageViewModel = new ConnectionPageViewModel(this);
            mappingPageViewModel = new MappingPageViewModel(this);
            importPageViewModel = new ImportPageViewModel(this);
        }

        public WizardWindow WizardWindow
        {
            get { return wizardWindow; }
            set { wizardWindow = value; }
        }

        public ConnectionPageViewModel ConnectionPageViewModel
        {
            get { return connectionPageViewModel; }
        }

        public MappingPageViewModel MappingPageViewModel
        {
            get { return mappingPageViewModel; }
        }

        public ImportPageViewModel ImportPageViewModel
        {
            get { return importPageViewModel; }
        }

        public void ChangeDatabase()
        {
            if (mappingPageViewModel.TableMappingViewModels != null && connectionPageViewModel.SelectedDatabase != null)
            {
                mappingPageViewModel.tableMappingSwitchDatabase(mappingPageViewModel.TableMappings.ToArray(), 
                    connectionPageViewModel.SelectedDatabase);
            }
            NotifyPropertyChanged("TableMappings");
            NotifyPropertyChanged("MappingTableReferences");
            NotifyPropertyChanged("TableMappingViewModels");
        }

        public string ConfigurationPath
        {
            get { return configurationPath; }
            set
            {
                configurationPath = value;
                NotifyPropertyChanged("ConfigurationPath");
            }
        }

        internal void SaveConfiguration()
        {
            try
            {
                TableMapping[] tableMappings = mappingPageViewModel.TableMappingViewModels.Select(t => t.TableMapping).ToArray();

                ImportConfiguration config = new ImportConfiguration(tableMappings, connectionPageViewModel.DatabaseConnector.ConnectionSetup,
                    connectionPageViewModel.SelectedDatabase.Name, importPageViewModel.ErrorHandling);

                ConfigurationSaver saver = new ConfigurationSaver(config, configurationPath);
                saver.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal void LoadConfiguration(string filePath)
        {
            try
            {
                ConfigurationPath = filePath;
                ConfigurationLoader loader = new ConfigurationLoader(configurationPath);
                ImportConfiguration config = loader.Load();

                connectionPageViewModel.ConnectionSetup = config.ConnectionSetup;
                connectionPageViewModel.SelectedDatabaseName = config.DatabaseName;

                if (connectionPageViewModel.SelectedDatabase != null)
                {
                    mappingPageViewModel.tableMappingSwitchDatabase(config.TableMappings, 
                        connectionPageViewModel.SelectedDatabase);
                }

                importPageViewModel.ErrorHandling = config.ErrorHandling;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
