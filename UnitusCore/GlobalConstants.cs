using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitusCore
{
    public static class GlobalConstants
    {
        public static string AdminRoleName = "Administrator";

        public static string[] ProtectedRoleNames=new string[] {"Administrator"};

        public const string CorsOrigins= "http://localhost:8888,http://localhost:3672,http://unitus-ac.com";
    }
}