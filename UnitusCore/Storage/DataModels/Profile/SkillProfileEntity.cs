using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels.Profile
{
    public interface ISkillProfile
    {
        string SkillName { get; set; }
        SkillLevel SkillLevel { get; set; }
    }

    class SkillProfileContainer : ISkillProfile
    {
        public string SkillName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SkillLevel SkillLevel { get; set; }
    }

    public class SkillProfileEntity:TableEntity, ISkillProfile
    {
        private string _skillName;

        public SkillProfileEntity()
        {
            
        }

        public SkillProfileEntity(string userId,string skillName,SkillLevel skillLevel)
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

        public int SkillLevelAsInt { get; set; }

        [IgnoreProperty]
        public SkillLevel SkillLevel
        {
            get { return (SkillLevel) SkillLevelAsInt; }
            set { SkillLevelAsInt = (int) value; }
        }

        [IgnoreProperty]
        public string UserId
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }
    }

    public interface ISkillInfo
    {
        string SkillName { get; set; }
    }

    class SkillInfoContainer : ISkillInfo
    {
        public string SkillName { get; set; }
    }

    /// <summary>
    /// 言語情報など
    /// </summary>
    public class SkillInfoEntity : TableEntity, ISkillInfo
    {
        private string _skillName;

        public SkillInfoEntity()
        {
            
        }

        public SkillInfoEntity(string skillName)
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