using System;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Statistics : ModelBase
    {
        public DateTime StatDate { get; set; }

        public int SumCircles { get; set; }

        public int SumPeoples { get; set; }
    }
}