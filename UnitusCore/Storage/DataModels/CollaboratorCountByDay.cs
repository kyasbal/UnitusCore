using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels
{
    public class CollaboratorCountByDay:TableEntity
    {
        public CollaboratorCountByDay()
        {
            
        }

        public CollaboratorCountByDay(string id,string gitLogin,int count)
        {
            Count = count;
            PartitionKey = id;
            RowKey = gitLogin;
        }

        public int Count { get; set; }

        [IgnoreProperty]
        public string TargetGitLogin
        {
            get { return RowKey; }
        }
    }
}