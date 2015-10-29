using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.DataReader
{
    public abstract class IDataReader
    {

        public abstract SourceDataTable ReadToDataTable();

        public abstract SourceDataTable ReadFirstRowToDataTable();

        public abstract string[] GetHeaderNames();
    }
}
