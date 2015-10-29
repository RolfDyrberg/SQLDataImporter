using NUnit.Framework;
using SQLDataImporter.DataReader;
using SQLDataImporter.StatementCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.Test
{
    [TestFixture]
    public class SQLServerDataEntryTranslatorTest
    {

        [TestCase]
        public void BoolTest()
        {
            SourceDataEntry trueEntry = SourceDataEntry.CreateDataEntry("1", DataType.Bool, "");
            SourceDataEntry falseEntry = SourceDataEntry.CreateDataEntry("0", DataType.Bool, "");

            Assert.AreEqual("1", SQLServerDataEntryTranslator.Translate(trueEntry));
            Assert.AreEqual("0", SQLServerDataEntryTranslator.Translate(falseEntry));
        }

        [TestCase]
        public void DateTimeTest()
        {
            SourceDataEntry datetimeEntry = SourceDataEntry.CreateDataEntry("2015-12-26 23:59:59", DataType.DateTime, "");
            Assert.AreEqual("'2015-12-26 23:59:59'", SQLServerDataEntryTranslator.Translate(datetimeEntry));
        }

        [TestCase]
        public void ErrorTest()
        {
            SourceDataEntry errorEntry = SourceDataEntry.CreateDataEntry("Test", DataType.Error, "");
            Assert.AreEqual("NULL", SQLServerDataEntryTranslator.Translate(errorEntry));
        }


        [TestCase]
        public void NullTest()
        {
            SourceDataEntry nullEntry = SourceDataEntry.CreateDataEntry("Test", DataType.Null, "");
            Assert.AreEqual("NULL", SQLServerDataEntryTranslator.Translate(nullEntry));
        }

        [TestCase]
        public void NumberTest()
        {
            SourceDataEntry numberEntry = SourceDataEntry.CreateDataEntry("1.23", DataType.Number, "");
            Assert.AreEqual("1.23", SQLServerDataEntryTranslator.Translate(numberEntry));
        }

        [TestCase]
        public void StringTest()
        {
            SourceDataEntry stringEntry1 = SourceDataEntry.CreateDataEntry("Test", DataType.String, "");
            SourceDataEntry stringEntry2 = SourceDataEntry.CreateDataEntry("Test's", DataType.String, "");
            Assert.AreEqual("'Test'", SQLServerDataEntryTranslator.Translate(stringEntry1));
            Assert.AreEqual("'Test''s'", SQLServerDataEntryTranslator.Translate(stringEntry2));
        }



    }
}
