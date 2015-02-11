using System;

namespace UnitusCore.Models.BaseClasses
{
    public abstract class ModelBaseWithTimeLogging :ModelBase
    {
        public DateTime LastModefied { get; set; }

        public DateTime CreationDate { get; set; }
    }
}