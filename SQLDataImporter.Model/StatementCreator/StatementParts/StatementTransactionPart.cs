/*
 * 
 * StatementTransactionPart proides the transaction handling for an ImportStatement
 * 
 */


using SQLDataImporter.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.StatementCreator
{
    public class StatementTransactionPart
    {

        private ErrorHandling errorHandling;

        public StatementTransactionPart(ImportConfiguration config)
        {
            errorHandling = config.ErrorHandling;
        }

        public string GetTransactionStartPart()
        {
            return errorHandling.ImportAsTransaction ? "BEGIN TRANSACTION [ImportTransaction]\nBEGIN TRY\n" : "";
        }

        public string GetTransactionEndPart()
        {
            return errorHandling.ImportAsTransaction ? transactionEndPart() : "";
        }

        private string transactionEndPart()
        {
            return string.Format("{0}\n\t{1}\n{2}\n\t{3}\n{4}\n{5}\n{6}\n{7}",
                "COMMIT TRANSACTION [ImportTransaction]", "END TRY", "BEGIN CATCH", "ROLLBACK TRANSACTION [ImportTransaction]\n",
                "\tDECLARE @ErrorMessage NVARCHAR(MAX)\n\tDECLARE @ErrorSeverity INT\n\tDECLARE @ErrorState INT\n",
                "\tSET @ErrorMessage = ERROR_MESSAGE()\n\tSET @ErrorSeverity = ERROR_SEVERITY()\n\tSET @ErrorState = ERROR_STATE()\n",
                "\tRAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)", "END CATCH");
        }

    }
}
