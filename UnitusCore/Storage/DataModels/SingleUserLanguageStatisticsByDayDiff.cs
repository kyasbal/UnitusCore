using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels
{
    public class SingleUserLanguageStatisticsByDayDiff:TableEntity
    {
        private int GetDiff(Func<SingleUserLanguageStatisticsByDay, int> f,SingleUserLanguageStatisticsByDay today,SingleUserLanguageStatisticsByDay last)
        {
            return f(today) - f(last);
        }

        private double GetDiff(Func<SingleUserLanguageStatisticsByDay, double> f, SingleUserLanguageStatisticsByDay today, SingleUserLanguageStatisticsByDay last)
        {
            return f(today) - f(last);
        }

        public SingleUserLanguageStatisticsByDayDiff()
        {
            
        }

        public SingleUserLanguageStatisticsByDayDiff(string uniqueId,SingleUserLanguageStatisticsByDay today,SingleUserLanguageStatisticsByDay last)
        {
            if(today.Language!=last.Language)throw new InvalidDataException();
            this.RowKey = uniqueId;
            this.Language = today.Language;
            SumCommitDiff = GetDiff(c => c.SumCommit, today, last);
            SumAdditionDiff = GetDiff(c => c.SumAddition, today, last);
            SumDeletionDiff = GetDiff(c => c.SumDeletion, today, last);
            SumRepositoryDiff= GetDiff(c => c.SumRepository, today, last);
            CommitRateDiff= GetDiff(c => c.CommitRateByLanguage, today, last);
            AdditionRateDiff= GetDiff(c => c.AdditionRateByLanguage, today, last);
            DeletionRateDiff= GetDiff(c => c.DeletionRateByLanguage, today, last);
            RepositoryRateDiff = GetDiff(c => c.RepositoryRateByLanguage, today, last);
        }

        public int SumCommitDiff { get; set; }

        public int SumAdditionDiff { get; set; }

        public int SumDeletionDiff { get; set; }

        public int SumRepositoryDiff { get; set; }

        public double CommitRateDiff { get; set; }

        public double AdditionRateDiff { get; set; }

        public double DeletionRateDiff { get; set; }

        public double RepositoryRateDiff { get; set; }

        [IgnoreProperty]
        public string Language
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }
    }
}