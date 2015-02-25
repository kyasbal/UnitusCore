using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels
{
    public class GistStatisticsForSingleUser:TableEntity
    {
        public GistStatisticsForSingleUser(string userId,int sumGistCount,int sumForked,long sumSize,int sumComments)
        {
            PartitionKey = userId;
            RowKey = DateTime.Now.ToDateCode().ToString();
            SumGistCount = sumGistCount;
            SumForked = sumForked;
            SumSize = sumSize;
            SumComments = sumComments;
        }

        public int SumGistCount { get; set; }

        public int SumForked { get; set; }

        public long SumSize { get; set; }

        public int SumComments { get; set; }
    }
}