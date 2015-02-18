using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels
{
    public class SingleUserLanguageStatisticsByDay:TableEntity
    {
        public SingleUserLanguageStatisticsByDay()
        {
            
        }

        public SingleUserLanguageStatisticsByDay(string uniqueId, string language, int sumAddition, int sumDeletion,
            int sumCommit, int sumRepository,double additionRateByLanguage,double deletionRateByLanguage,double commitRateByLanguage,double repositoryRateByLanguage)
        {
            SumAddition = sumAddition;
            SumDeletion = sumDeletion;
            SumCommit = sumCommit;
            SumRepository = sumRepository;
            AdditionRateByLanguage = additionRateByLanguage;
            DeletionRateByLanguage = deletionRateByLanguage;
            CommitRateByLanguage = commitRateByLanguage;
            RepositoryRateByLanguage = repositoryRateByLanguage;
            RowKey = uniqueId;
            PartitionKey = language;
            
        }

        public int SumAddition { get; set; }

        public int SumDeletion { get; set; }

        public int SumCommit { get; set; }

        public int SumRepository { get; set; }

        public double AdditionRateByLanguage { get; set; }

        public double DeletionRateByLanguage { get; set; }

        public double CommitRateByLanguage { get; set; }

        public double RepositoryRateByLanguage { get; set; }

        [IgnoreProperty]
        public string Language
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }
    }
}