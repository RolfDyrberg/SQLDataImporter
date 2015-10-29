using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DatabaseConnector
{
    
    public interface IDatabaseConnector
    {

        ConnectionSetup ConnectionSetup
        {
            get;
        }

        List<string> GetDatabaseNames();

        Database GetDatabase(string databaseName);



    }
}
