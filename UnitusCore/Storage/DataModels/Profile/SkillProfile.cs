using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels.Profile
{
    public class SkillProfile:TableEntity
    {
        private string _skillName;

        public SkillProfile()
        {
            
        }

        public SkillProfile(string userId,string skillName,SkillLevel skillLevel)
        {
            UserId = userId;
            SkillName = skillName;
            SkillLevel = skillLevel;
        }

        public string SkillName
        {
            get { return _skillName; }
            set
            {
                _skillName = value;
                RowKey = value.ToHashCode();
            }
        }

        public SkillLevel SkillLevel { get; set; }

        [IgnoreProperty]
        public string UserId
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }
    }

    /// <summary>
    /// 言語情報など
    /// </summary>
    public class SkillInfo : TableEntity
    {
        private string _skillName;

        public SkillInfo()
        {
            
        }

        public SkillInfo(string skillName)
        {
            PartitionKey = "SKILL";
            SkillName = skillName;
        }


        public string SkillName
        {
            get { return _skillName; }
            set
            {
                _skillName = value;
                RowKey = value.ToHashCode();
            }
        }
    }

    public enum SkillLevel
    {
        Interested,
        Learning,
        WellTrained,
        ProudOf
    }
}