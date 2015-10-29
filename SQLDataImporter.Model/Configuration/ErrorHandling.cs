/*
 * 
 * ErrorHadling is part of an ImportConfiguration - it describes how errors should be handled during the import.
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.Configuration
{
    public class ErrorHandling
    {

        private bool ignoreWarnings;
        private bool outputRowNumbers;
        private bool importAsTransaction;

        public ErrorHandling()
        {
            ignoreWarnings = false;
            outputRowNumbers = true;
            importAsTransaction = true;
        }

        public ErrorHandling(bool ignoreWarnings, bool outputRowNumbers, bool importAsTransaction)
        {
            this.ignoreWarnings = ignoreWarnings;
            this.outputRowNumbers = outputRowNumbers;
            this.importAsTransaction = importAsTransaction;
        }

        public bool IgnoreWarnings
        {
            get { return ignoreWarnings; }
            set { ignoreWarnings = value; }
        }

        public bool OutputRowNumbers
        {
            get { return outputRowNumbers; }
            set { outputRowNumbers = value; }
        }

        public bool ImportAsTransaction
        {
            get { return importAsTransaction; }
            set { importAsTransaction = value; }
        }
    }
}
