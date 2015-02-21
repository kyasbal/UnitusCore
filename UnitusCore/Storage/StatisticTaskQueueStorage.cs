using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using Microsoft.WindowsAzure.Storage.Queue;
using UnitusCore.Controllers;
using UnitusCore.Storage.Base;

namespace UnitusCore.Storage
{
    public class StatisticTaskQueueStorage
    {
        private static Dictionary<QueuedTaskType,string> QueueNameDictionary=new Dictionary<QueuedTaskType, string>()
        {
            {QueuedTaskType.SingleUserAchivementStatistics, "singleuser-achivement-stat"},
            {QueuedTaskType.SingleUserContributionStatistics, "singleuser-contribution-stat"},
            {QueuedTaskType.SystemAchivementStatistics, "system-achivement-stat"},
            {QueuedTaskType.CircleAchivementStatistics, "circle-achivement-stat"},
            {QueuedTaskType.FinalizeAchivementStatistics,"finalize-achivement-stat"},
            {QueuedTaskType.PreInitializeForGithub, "preinitialize-github-stat"}
        }; 

        private Dictionary<QueuedTaskType,CloudQueue> QueueDictionary=new Dictionary<QueuedTaskType, CloudQueue>(); 

        private readonly QueueStorageConnection _connection;

        public StatisticTaskQueueStorage(QueueStorageConnection connection)
        {
            _connection = connection;
            foreach (KeyValuePair<QueuedTaskType, string> queueName in QueueNameDictionary)
            {
                var queue=_connection.Client.GetQueueReference(queueName.Value);
                queue.CreateIfNotExists();
                QueueDictionary.Add(queueName.Key,queue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="count"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public async Task<bool> CheckNeedOfFinishedTaskExecuteWhenExisiting(QueuedTaskType taskType, int count,
            Action<IEnumerable<QueueMessageContainer>> f)
        {
            var queuedTasks = BeginTask(count, taskType);
            if (queuedTasks.Any())
            {
                f(queuedTasks);
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> CheckNeedOfFinishedTaskExecuteWhenExisiting(StringBuilder builder, QueuedTaskType taskType, int count, Func<IEnumerable<QueueMessageContainer>, Task> f)
        {
            
            var queuedTasks = BeginTask(count, taskType);
            if (queuedTasks.Any())
            {
                var log = string.Format("{0} TaskType:{1} Count:{2}/{3}(Actual/Request)\n",DateTime.Now,taskType,queuedTasks.Count(),count);
                foreach (QueueMessageContainer tasks in queuedTasks)
                {
                    log += string.Format("TargetAddr:{0} Argument:{1}\n", tasks.TargetAddress, tasks.TargetArguments);
                }
                builder.Append(log);
                await f(queuedTasks);
                return false;
            }
            else
            {
                return true;
            }
        }


        public async Task<int> GetQueueLength(QueuedTaskType taskType)
        {
            await QueueDictionary[taskType].FetchAttributesAsync();
            int? count = QueueDictionary[taskType].ApproximateMessageCount;
            return count.GetValueOrDefault();
        }

        public async Task<bool> HasTask(QueuedTaskType taskType)
        {
            return await GetQueueLength(taskType) > 0;
        }

        public async Task AddQueue(QueuedTaskType taskType,string targetAdddress,object arguments)
        {
            CloudQueueMessage message=new CloudQueueMessage(Json.Encode(new QueueMessageContainer(targetAdddress,Json.Encode(arguments),taskType)));
            await QueueDictionary[taskType].AddMessageAsync(message);
        }

        public IEnumerable<QueueMessageContainer> BeginTask(int count,QueuedTaskType taskType)
        {
            return QueueDictionary[taskType].GetMessages(count, TimeSpan.FromMinutes(5)).Select(a => Json.Decode<QueueMessageContainer>(a.AsString).SetContainingQueue(a));
        }

        public async Task EndTask(QueueMessageContainer queue)
        {
            await QueueDictionary[queue.TaskType].DeleteMessageAsync(queue.ContainingQueue);
        }

        public class QueueMessageContainer
        {
            public QueueMessageContainer()
            {
            }

            public QueueMessageContainer(string targetAddress, string targetArguments, QueuedTaskType taskType)
            {
                TargetAddress = targetAddress;
                TargetArguments = targetArguments;
                TaskType = taskType;
            }

            public string TargetAddress { get; set; }

            public string TargetArguments { get; set; }

            public CloudQueueMessage ContainingQueue { get; set; }

            public QueuedTaskType TaskType { get; set; }

            public QueueMessageContainer SetContainingQueue(CloudQueueMessage queueMessage)
            {
                this.ContainingQueue = queueMessage;
                return this;
            }
        }

        public enum QueuedTaskType
        {
            SingleUserContributionStatistics,
            SingleUserAchivementStatistics,
            SystemAchivementStatistics,
            CircleAchivementStatistics,
            FinalizeAchivementStatistics,
            PreInitializeForGithub
        }
    }
}