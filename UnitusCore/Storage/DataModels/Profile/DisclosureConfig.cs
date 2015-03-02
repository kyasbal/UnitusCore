using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels.Profile
{
    public class DisclosureConfig:TableEntity
    {
        public DisclosureConfig()
        {
            
        }

        public DisclosureConfig(string userId,ProfileProperty prop,ProfilePropertyDisclosureConfig disclosureConfig)
        {
            PartitionKey = userId;
            RowKey = prop.ToString();
            Config = disclosureConfig;
        }

        public string ConfigString { get; set; }

        [IgnoreProperty]
        public ProfilePropertyDisclosureConfig Config
        {
            get
            {
                return
                    (ProfilePropertyDisclosureConfig) Enum.Parse(typeof (ProfilePropertyDisclosureConfig), ConfigString);
            }
            set { ConfigString = value.ToString(); }
        }
    }

    [Flags]
    public enum ProfilePropertyDisclosureConfig
    {
        Private=0x01,
        CircleOnly=0x03,
        Public=0x07
    }

    public enum ProfileProperty
    {
        Language,
        University,
        Faculty,
        Major,
        MailAddress
    }
}