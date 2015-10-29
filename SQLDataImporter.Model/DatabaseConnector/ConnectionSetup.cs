/*
 * 
 * ConnectionSetup describes how to connection to a SQL Server database.
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DatabaseConnector
{
    public class ConnectionSetup
    {

        private string serverName;
        private string userName;
        private string password;
        private bool useWindowsAuthentication;

        private int timeout = 10;

        public ConnectionSetup(string serverName, string userName, string password, bool useWindowsAuthentication)
        {
            this.serverName = serverName;
            this.userName = userName;
            this.password = password;
            this.useWindowsAuthentication = useWindowsAuthentication;
        }

        public ConnectionSetup(string serverName, string userName, string password, bool useWindowsAuthentication, int timeout)
        {
            this.serverName = serverName;
            this.userName = userName;
            this.password = password;
            this.useWindowsAuthentication = useWindowsAuthentication;
            this.timeout = timeout;
        }

        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public bool UseWindowsAuthentication
        {
            get { return useWindowsAuthentication; }
            set { useWindowsAuthentication = value; }
        }

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

    }
}
