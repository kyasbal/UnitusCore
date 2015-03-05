using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels
{
    public class StringResource:TableEntity
    {
        public StringResource()
        {
        }

        public StringResource(string propName,string body)
        {
            PartitionKey = CultureInfo.GetCultureInfo("JA-JP").NativeName;
            RowKey = propName;
            Body = body;
        }

        [IgnoreProperty]
        public string PropertyName
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

        public string Body { get; set; }

    }
}