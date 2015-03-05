using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels.Circle
{
    public class CircleMemberTagBody:TableEntity
    {
        public CircleMemberTagBody()
        {
            
        }
        private string _tag;

        public CircleMemberTagBody(string circleId,string tag,bool isVisibleForNonAdmin)
        {
            PartitionKey = circleId;
            Tag = tag;
            IsVisibleForNonAdmin = isVisibleForNonAdmin;
        }

        public string Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                RowKey = value.ToHashCode();
            }
        }

        public bool IsVisibleForNonAdmin { get; set; }
    }
}