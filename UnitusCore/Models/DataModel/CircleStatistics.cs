using System;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class CircleStatistics : ModelBase
    {
        public int GithubUserCount { get; set; }

        public int RepositoryCount { get; set; }

        public int CommitCount { get; set; }

        public double CommitPerUser { get; set; }

        public Circle RelatedCircle { get; set; }

        public DateTime StatDate { get; set; }
    }
}