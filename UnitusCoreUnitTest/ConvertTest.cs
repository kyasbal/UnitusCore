using System;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Core;
using NUnit.Framework;
using UnitusCore.Controllers;
using UnitusCore.Controllers.Misc;
using UnitusCore.Models.DataModel;

namespace UnitusCoreUnitTest
{
    [TestFixture]
    public class ConvertTest
    {
        [Test]
        public void CircleConvertTest()
        {
            Circle dummyCircle = DummyFactory.GenerateDummyCircleData();
            CircleController.AddCircleRequest req=new CircleController.AddCircleRequest();
            Mapper.Map(dummyCircle, req);
            MapperHelper.DebugForProperty(dummyCircle);
            MapperHelper.DebugForProperty(req);
        }
    }
}
