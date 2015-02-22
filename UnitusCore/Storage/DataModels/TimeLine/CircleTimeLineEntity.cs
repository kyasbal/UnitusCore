using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels.TimeLine
{
    public class TimeLineEntity:TableEntity
    {
        public TimeLineEntity()
        {
            
        }

        public TimeLineEntity(TimeLineType type,string lineId)
        {
            
        }

        public enum TimeLineType
        {
            Circle,Person
        }

        public enum FormatterType
        {
            GotAchivement,
        }
    }

    public abstract class TimeLineEntityFormatter
    {
        public abstract string GetFormattedString(string argument);
    }
}