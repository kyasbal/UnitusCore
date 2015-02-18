using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UnitusCore.Models;

namespace UnitusCore.Migrations
{
    public class CustomDbInitializer:IDatabaseInitializer<ApplicationDbContext>

{
        public void InitializeDatabase(ApplicationDbContext context)
        {
            
        }
}
}