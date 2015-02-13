using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using UnitusCore.Models;

namespace UnitusCoreUnitTest
{
    class TestingDbConfiguration:DbConfiguration
    {
        public TestingDbConfiguration()
        {
           SetProviderServices("System.Data.SqlClient", SqlProviderServices.Instance);
        }
    }

    class TestingMigrationDbConfiguration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public TestingMigrationDbConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
