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

        public const string CorsOrigins = "https://localhost:8888,https://localhost:3672,https://localhost:44301,http://unitus-ac.com,https://core.unitus-ac.com,http://core.unitus-ac.com";
    }
}