using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UnitusCore.Attributes;

namespace UnitusCore.Controllers
{
    public class TutorialController:UnitusApiController
    {
        [HttpGet]
        [Route("Tutorial/Test")]
        [UnitusCorsEnabled]
        public async Task<IHttpActionResult> Test()
        {
            var users=DbSession.Users.Select(a => a.UserName).ToArray();
            return Json(users);
        }

        public class TestRequestModel
        {
            public TestRequestModel()
            {
            }

            public string TestProperty1 { get; set; }

            public string TestProperty2 { get; set; }

            public TestRequestModel(string testProperty1, string testProperty2)
            {
                TestProperty1 = testProperty1;
                TestProperty2 = testProperty2;
            }

            public override string ToString()
            {
                return string.Format("TestProperty1: {0}, TestProperty2: {1}", TestProperty1, TestProperty2);
            }
        }
    }
}
