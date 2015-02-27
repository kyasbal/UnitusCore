using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels.Circle
{
    public class CircleMemberTagGlue:TableEntity
    {
        private string _gluedValue;

        public static string GeneratePartitionKey(string circleId, string glue)
        {
            return circleId + "-" + glue.ToHashCode();
        }

        public string GluedValue
        {
            get { return _gluedValue; }
            set
            {
                _gluedValue = value;
                RowKey = value.ToHashCode();
            }
        }
    }
}