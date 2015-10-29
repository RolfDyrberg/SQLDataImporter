/*
 * 
 * ConnectionStringMaker provides a connectionstring, which is used to connect to a SQL Server database.
 * The basis for these strings can be found here: http://www.connectionstrings.com/sql-server/
 * 
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DatabaseConnector
{
    public class ConnectionStringMaker
    {

        public static string SQLServerConnectionString(ConnectionSetup connectionSetup)
        {
            if (connectionSetup.UseWindowsAuthentication)
            {
                return String.Format("server={0};Integrated Security=SSPI;Trusted_Connection=yes;connection timeout={1}", 
                    connectionSetup.ServerName, connectionSetup.Timeout);
            }
            else
            {
                return String.Format("user id={0};password={1};server={2};Trusted_Connection=no;connection timeout={3}", 
                    connectionSetup.UserName, connectionSetup.Password, connectionSetup.ServerName, connectionSetup.Timeout);
            }
        }

    }



}
