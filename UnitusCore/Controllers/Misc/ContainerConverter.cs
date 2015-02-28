using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace UnitusCore.Controllers.Misc
{
    public static class ContainerConverter
    {
        public static void Convert<T, TI>(T arg1, TI arg2) where TI:T where T:class
        {
            var properties=typeof(T).GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                if(!prop.CanRead)continue;
                var targetProperty=typeof (TI).GetProperty(prop.Name);
                if(!targetProperty.CanRead)continue;
                targetProperty.SetValue(arg2,prop.GetValue(arg1));
            }
        }

        public static void DebugForProperty<T>(T arg) where T : class
        {
            Console.WriteLine("class:{0}",typeof(T).FullName);
            var properties = typeof (T).GetProperties();
            foreach (var prop in properties)
            {
                if(!prop.CanRead)continue;
                Console.WriteLine("{0}:{1}",prop.Name,prop.GetValue(arg));
            }
        }
    }
}