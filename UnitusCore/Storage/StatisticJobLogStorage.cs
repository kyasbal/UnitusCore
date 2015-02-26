using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;

namespace UnitusCore.Storage
{
    public class StatisticJobLogStorage
    {
        private const string DailyStatisticsJobLogTableName = "DailyStatisticsJobLog";

        private readonly CloudTable _dailyStatisticsJobLogTable;

        public StatisticJobLogStorage(TableStorageConnection connection)
        {
            this._dailyStatisticsJobLogTable = connection.TableClient.GetTableReference(DailyStatisticsJobLogTableName);
            _dailyStatisticsJobLogTable.CreateIfNotExists();
        }

        public async Task Add(DailyStatisticJobAction jobType, string argument, double taskTime, string log)
        {
            TableOperation insertOperation = TableOperation.InsertOrReplace(new DailyStatisticsJobLog(jobType,argument,taskTime,log));
            await _dailyStatisticsJobLogTable.ExecuteAsync(insertOperation);
        }

        public void AddNSync(DailyStatisticJobAction jobType, string argument, double taskTime, string log)
        {
            TableOperation insertOperation = TableOperation.InsertOrReplace(new DailyStatisticsJobLog(jobType, argument, taskTime, log));
            _dailyStatisticsJobLogTable.Execute(insertOperation);
        }

        public StatisticJobLogger GetLogger()
        {
            return new StatisticJobLogger(this);
        }

        public class StatisticJobLogger
        {
            private readonly StatisticJobLogStorage _storage;

            public StatisticJobLogger(StatisticJobLogStorage storage)
            {
                _storage = storage;
            }

            private Stopwatch _stopwatch;
            private DailyStatisticJobAction _jobType;
            private string _argument;

            public StatisticJobLogger Begin(DailyStatisticJobAction jobType,string argument)
            {
                _argument = argument;
                _jobType = jobType;
                _stopwatch=new Stopwatch();
                _stopwatch.Start();
                return this;
            }

            public async Task End(string log)
            {
                _stopwatch.Stop();
                await _storage.Add(_jobType, _argument, _stopwatch.ElapsedMilliseconds, log);
            }

            public void EndNSync(string log)
            {
                _stopwatch.Stop();
                _storage.AddNSync(_jobType, _argument, _stopwatch.ElapsedMilliseconds, log);
            }
        }
    }
}