using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels
{
    public class GistStatisticsForSingleUserByLanguage:TableEntity,IObjectDictionaryEntity
    {
        public GistStatisticsForSingleUserByLanguage(string userId,string languageName,int bytes)
        {
            PartitionKey = languageName;
            RowKey = GetRowKey(userId, DateTime.Now);
            FileInBytes = bytes;
        }

        [IgnoreProperty]
        public string Name
        {
            get
            {
                return PartitionKey;
            }
            set { PartitionKey = value; }
        }

        public int FileInBytes { get; set; }

        public static string GetRowKey(string userId,DateTime dateCode)
        {
            return GetRowKey(userId, dateCode.ToDateCode());
        }

        public static string GetRowKey(string userId, long dateCode)
        {
            return userId + "-" + dateCode;
        }
    }
}