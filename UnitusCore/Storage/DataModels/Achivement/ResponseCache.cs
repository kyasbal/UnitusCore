using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels.Achivement
{
    public class ResponseCache:TableEntity
    {
        public ResponseCache()
        {
            
        }

        public ResponseCache(string achivementName,string userId,string json)
        {
            CacheType=ResponseCacheType.Basic;
            PartitionKey = "ResponseCache";
            RowKey = GetRowKey(achivementName, userId, CacheType);
            CachedResponse = json;
        }

        public ResponseCacheType CacheType { get; set; }

        public string CachedResponse { get; set; }



        public static string GetRowKey(string achivementName,string userId, ResponseCacheType type)
        {
            return achivementName + "-" + type+"-"+userId;
        }

        public enum ResponseCacheType
        {
            Basic
        }
    }
}