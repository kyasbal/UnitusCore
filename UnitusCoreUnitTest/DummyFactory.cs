using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitusCore.Models.DataModel;
using UnitusCore.Util;

namespace UnitusCoreUnitTest
{
    public static class DummyFactory
    {
        private static Random rand;

        public static Random Random
        {
            get
            {
                rand = rand ?? new Random();
                return rand;
            }
        }

        public static Circle GenerateDummyCircleData()
        {
            Circle circle=new Circle()
            {
                ActivityDate = IdGenerator.GetId(20),
                Id = Guid.NewGuid(),
                Name = IdGenerator.GetId(20),
                BelongedSchool = IdGenerator.GetId(20),
                CanInterColledge = false,
                Contact = IdGenerator.GetId(20),
                Notes = IdGenerator.GetId(20),
                WebAddress = IdGenerator.GetId(20),
                MemberCount = Random.Next(),
                LastModefied = DateTime.Now,
                CreationDate = DateTime.Now,
                Description = IdGenerator.GetId(200),
            };
            return circle;
        }
    }
}
