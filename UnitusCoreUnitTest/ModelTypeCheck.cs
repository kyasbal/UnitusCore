using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitusCore.Models;
using UnitusCore.Models.BaseClasses;
using UnitusCore.Models.DataModel;

namespace UnitusCoreUnitTest
{
    [TestClass]
    public class ModelTypeCheck
    {
        public ModelTypeCheck()
        {
        }

        private void createModelTypesIfNotexist()
        {
            if (ModelTypes == null)
            {
                Assembly asmAssembly = Assembly.GetAssembly(typeof(ModelBase));
                ModelTypes = new HashSet<Type>(asmAssembly.GetTypes().Where(a => a.IsSubclassOf(typeof(ModelBase))));
                ModelTypes.Add(typeof(ApplicationUser));
            }
        }

        private HashSet<Type> ModelTypes;

        private ApplicationDbContext DbSession;

        [TestInitialize]
        public void TestPrepare()
        {
            createModelTypesIfNotexist();
            DbSession=new ApplicationDbContext(@"Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=C:\Users\Lime\Source\Repos\unitus-core\UnitusCore\App_Data\UnitusCoreDatabase.mdf;Initial Catalog=UnitusCoreDatabase;Integrated Security=True");
        }


        [TestMethod]
        public void HasSimpleConstructor()
        {
            bool hasSimpleConstructor = true;
            foreach (var modelType in ModelTypes)
            {
                int count = 1;
                var constructor=modelType.GetConstructors(BindingFlags.Public);
                if (constructor.Length == 0) count = 0;
                foreach (ConstructorInfo constructorInfo in constructor)
                {
                    count = Math.Min(count, constructorInfo.GetParameters().Length);
                    Console.WriteLine("Type:{0} count:{1}", modelType, count);
                }
                if (count != 0)
                {
                    hasSimpleConstructor = false;
                }
                if (hasSimpleConstructor)
                {
                    Console.WriteLine("Type{0} is OK",modelType);
                }
                else
                {
                    Console.WriteLine("Type{0} is Invalid", modelType);
                }
            }
            Assert.IsTrue(hasSimpleConstructor);
        }

        [TestMethod]
        public void ConnectionCheck()
        {
            Assert.IsNotNull(DbSession);
        }
        
    }
}
