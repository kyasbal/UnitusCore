using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(UnitusCore.Startup))]

namespace UnitusCore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            MvcHandler.DisableMvcResponseHeader = true;
            ConfigureAuth(app);
        }
    }
}
