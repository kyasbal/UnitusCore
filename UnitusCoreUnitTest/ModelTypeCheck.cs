using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public void HasSimpleConstructor()
        {
            createModelTypesIfNotexist();
            bool hasSimpleConstructor = true;
            foreach (var modelType in ModelTypes)
            {

                foreach (ConstructorInfo constructorInfo in modelType.GetConstructors(BindingFlags.Public))
                {
                    int count = constructorInfo.GetParameters().Length;
                    Console.WriteLine("Type:{0} count:{1}",modelType,count);
                    if (count != 0) hasSimpleConstructor = false;
                }
            }
            Assert.IsFalse(hasSimpleConstructor);
        }
    }
}
