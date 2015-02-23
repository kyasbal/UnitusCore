using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels.Achivement
{
    public class CategoryAchivementPair:TableEntity
    {
        public CategoryAchivementPair()
        {
            
        }

        public CategoryAchivementPair(string achivementName,string categoryName)
        {
            AchivementName = achivementName;
            CategoryName = categoryName;
        }

        [IgnoreProperty]
        public string AchivementName
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

        [IgnoreProperty]
        public string CategoryName
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }
    }

    public class CategoryNamesEntity : TableEntity
    {
        public CategoryNamesEntity()
        {
            
        }

        public CategoryNamesEntity(string categoryName)
        {
            PartitionKey = "CATEGORY";
            CategoryName = categoryName;
        }

        [IgnoreProperty]
        public string CategoryName
        {
            get { return RowKey; }
            set { RowKey = value; }
        }
    }
}