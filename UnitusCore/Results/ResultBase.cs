using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitusCore.Results
{
    public class ResultContainer<T>
    {
        public static ResultContainer<T> GenerateSuccessResult(T content)
        {
            ResultContainer<T> container=new ResultContainer<T>();
            container.Success = true;
            container.ErrorMessage = "Successful Completion";
            container.Content = content;
            return container;
        }

        public static ResultContainer<T> GenerateFaultResult(string errorMsg,T content)
        {
            ResultContainer<T> container = new ResultContainer<T>();
            container.Success = false;
            container.ErrorMessage = errorMsg;
            container.Content = content;
            return container;
        }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public T Content { get; set; }
    }
}