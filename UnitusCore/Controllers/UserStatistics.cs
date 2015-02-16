using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnitusCore.Models.BaseClasses;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Controllers
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