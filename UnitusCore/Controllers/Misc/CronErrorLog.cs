using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using UnitusCore.Storage;

namespace UnitusCore.Controllers.Misc
{
    public class CronErrorLog
    {
        public CronErrorLog(bool isSuccess,StatisticTaskQueueStorage.QueuedTaskType taskType, string address, string argumentJson, HttpStatusCode statusCode, string responseHeader, string responseBody)
        {
            IsSuccess = isSuccess;
            Address = address;
            TaskType = taskType;
            ArgumentJson = argumentJson;
            StatusCode = statusCode;
            ResponseHeader = responseHeader;
            ResponseBody = responseBody;
        }

        public bool IsSuccess { get; set; }

        public string Address { get; set; }

        public string ArgumentJson { get;set; }

        public StatisticTaskQueueStorage.QueuedTaskType TaskType { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string ResponseHeader { get; set; }

        public string ResponseBody { get; set; }
    }
}