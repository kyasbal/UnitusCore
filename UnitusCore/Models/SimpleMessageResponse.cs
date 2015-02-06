
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitusCore.Models
{
    public class SimpleMessageResponse
    {
        public SimpleMessageResponse(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}