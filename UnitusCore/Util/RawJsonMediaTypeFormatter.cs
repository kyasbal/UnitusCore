using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace UnitusCore.Util
{
    public class RawJsonMediaTypeFormatter:MediaTypeFormatter
    {
        public override bool CanReadType(Type type)
        {
            if (type.Equals(typeof (string))) return true;
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            if (type.Equals(typeof(string))) return true;
            return false;
        }

        public async override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            using (StreamWriter writer=new StreamWriter(writeStream))
            {
                await writer.WriteAsync((string) value);
            }
        }

        public async override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext, CancellationToken cancellationToken)
        {
            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                await writer.WriteAsync((string)value);
            }
        }
    }
}