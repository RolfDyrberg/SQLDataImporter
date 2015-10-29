using SQLDataImporter.Configuration;
using SQLDataImporter.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDataImporter.Test
{
    public class TestData
    {

        public static TableMapping[] TableMappingTestData()
        {
            DBTable addressTable = new DBTable("dbo", "Address");
            DBColumn a_idCol = new DBColumn(addressTable, "a_id", true, DBDatatype.integer);
            DBColumn streetNameCol = new DBColumn(addressTable, "StreetName", false, DBDatatype.nvarchar);
            DBColumn streetNumberCol = new DBColumn(addressTable, "StreetNumber", false, DBDatatype.integer);
            DBColumn zipCodeCol = new DBColumn(addressTable, "ZipCode", false, DBDatatype.integer);
            addressTable.Columns = new List<DBColumn>() { a_idCol, streetNameCol, streetNumberCol, zipCodeCol };

            NullColumnMapping a_idMapping = new NullColumnMapping(a_idCol, ColumnUse.Insert);
            ExcelColumnMapping streetNameMapping = new ExcelColumnMapping("Street name", streetNameCol, ColumnUse.Insert);
            ExcelColumnMapping streetNumberMapping = new ExcelColumnMapping("Street number", streetNumberCol, ColumnUse.Insert);
            ExcelColumnMapping zipCodeMapping = new ExcelColumnMapping("Zip code", zipCodeCol, ColumnUse.Insert);
            TableMapping addressTableMapping = new TableMapping(addressTable, TableMappingImportType.Insert, new ColumnMapping[] { a_idMapping, streetNameMapping, streetNumberMapping, zipCodeMapping });

            DBTable personTable = new DBTable("dbo", "Person");
            DBColumn p_idCol = new DBColumn(personTable, "p_id", true, DBDatatype.integer);
            DBColumn firstNameCol = new DBColumn(personTable, "FirstName", false, DBDatatype.nvarchar);
            DBColumn lastNameCol = new DBColumn(personTable, "LastName", false, DBDatatype.nvarchar);
            DBColumn a_idPersonCol = new DBColumn(personTable, "a_id", false, DBDatatype.integer);
            personTable.Columns = new List<DBColumn>() { p_idCol, firstNameCol, lastNameCol, a_idPersonCol };

            TableMapping personTableMapping = new TableMapping(personTable, TableMappingImportType.Insert, new ColumnMapping[0]);
            NullColumnMapping p_idMapping = new NullColumnMapping(p_idCol, ColumnUse.Insert);
            ExcelColumnMapping firstNameMapping = new ExcelColumnMapping("FirstName", firstNameCol, ColumnUse.Insert);
            ExcelColumnMapping lastNameMapping = new ExcelColumnMapping("Surname", lastNameCol, ColumnUse.Insert);
            TableColumnMapping aIdMapping = new TableColumnMapping(addressTableMapping, a_idMapping.DestinationColumn, a_idPersonCol, ColumnUse.Insert);
            personTableMapping.ColumnMappings = new ColumnMapping[] { p_idMapping, firstNameMapping, lastNameMapping, aIdMapping };


            DBTable contactInfoTable = new DBTable("dbo", "ContactInfo");
            DBColumn pn_idCol = new DBColumn(contactInfoTable, "pn_id", true, DBDatatype.integer);
            DBColumn textCol = new DBColumn(contactInfoTable, "text", false, DBDatatype.nvarchar);
            DBColumn p_idCICol = new DBColumn(contactInfoTable, "p_id", false, DBDatatype.integer);
            DBColumn ci_idCICol = new DBColumn(contactInfoTable, "ci_id", false, DBDatatype.integer);
            contactInfoTable.Columns = new List<DBColumn>() { pn_idCol, textCol, p_idCICol, ci_idCICol };


            ExcelColumnMapping phoneNumberMapping = new ExcelColumnMapping("Phone", textCol, ColumnUse.Insert);
            TableColumnMapping pIDMapping = new TableColumnMapping(personTableMapping, p_idMapping.DestinationColumn, p_idCICol, ColumnUse.Insert);
            LiteralColumnMapping citIdMapping = new LiteralColumnMapping("1", LiteralType.Integer, ci_idCICol, ColumnUse.Insert);
            TableMapping phoneTableMapping = new TableMapping(contactInfoTable, TableMappingImportType.Insert, new ColumnMapping[] { phoneNumberMapping, pIDMapping, citIdMapping });


            ExcelColumnMapping mobileNumberMapping = new ExcelColumnMapping("Mobile", textCol, ColumnUse.Insert);
            TableColumnMapping pIDMobileMapping = new TableColumnMapping(personTableMapping, p_idMapping.DestinationColumn, p_idCICol, ColumnUse.Insert);
            LiteralColumnMapping citIdMobileMapping = new LiteralColumnMapping("2", LiteralType.Integer, ci_idCICol, ColumnUse.Insert);
            TableMapping mobileTableMapping = new TableMapping(contactInfoTable, TableMappingImportType.Insert, new ColumnMapping[] { mobileNumberMapping, pIDMobileMapping, citIdMobileMapping });

            return new TableMapping[] { personTableMapping, phoneTableMapping, addressTableMapping, mobileTableMapping };
        }


    }
}
