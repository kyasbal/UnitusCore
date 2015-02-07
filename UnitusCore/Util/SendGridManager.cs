using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using SendGridSharp;

namespace UnitusCore.Util
{
    public class SendGridManager
    {
        private static readonly string FromAddr = "noreply@unitus-ac.com";

        private static readonly Dictionary<TemplateType,string> TypeIdDictionary=new Dictionary<TemplateType, string>()
        {
            {TemplateType.AccountConfirmation,"9a973b47-1d0b-42ec-bde8-126978847c39"} ,
            {TemplateType.AccountConfirmationCompleted, "0a215a48-69ba-491f-a60b-0079822801ef"}
        };
        public static void SendUseTemplate(string to,TemplateType type,Dictionary<string,string> args)
        {
            var Credentials = new NetworkCredential("sgvniwvl@kke.com", "wNTdzCtn%d");
            var client = new SendGridClient(Credentials);
            var message = new SendGridMessage();
            message.To.Add(to);
            message.From = FromAddr;
            foreach (var pair in args)
            {
                message.Header.AddSubstitution(pair.Key,pair.Value);
            }
            message.Text = " ";
            message.Subject = " ";
            message.Html = " ";
            message.UseTemplateEngine(TypeIdDictionary[type]);
            client.Send(message);
        }
    }

    public enum TemplateType
    {
        AccountConfirmation,
        AccountConfirmationCompleted
    }
}