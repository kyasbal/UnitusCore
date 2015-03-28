using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using AutoMapper;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.DataModels.Profile;

namespace UnitusCore.Controllers.Misc
{
    public static class MapperHelper
    {
        /// <summary>
        /// Mapを用いてデータを移す際のクラスの関係をここで初期化しておきます。
        /// </summary>
        public static void Initialize()
        {
            Mapper.CreateMap(typeof (Circle), typeof (CircleController.AddCircleRequest));
            Mapper.CreateMap(typeof (CircleController.AddCircleRequest), typeof (Circle));
            Mapper.CreateMap(typeof (DisclosureConfig), typeof (UserConfigureController.DisplayDisclosureConfig));
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