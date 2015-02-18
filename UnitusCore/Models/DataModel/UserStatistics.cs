using System;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class UserStatistics:ModelBase
    {
        public UserStatistics()
        {
            
        }
        
        public DateTime RecordedTime { get; set; }

        public Person LinkedPerson { get; set; }

        public int RepositoryCount { get; set; }

        public int CommitCount { get; set; }

        public int SumDeletion { get; set; }

        public int SumAddition { get; set; }

        public string LanguageRatioJson { get; set; }
    }


}