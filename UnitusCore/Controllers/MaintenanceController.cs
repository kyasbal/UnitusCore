using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;

namespace UnitusCore.Controllers
{
    public class MaintenanceController : UnitusApiController
    {
        [HttpPost]
        [Route("Maintenance/Achivements/AdjustBody")]
        public async Task<IHttpActionResult> GenerateToAdjustAchivementBody()
        {
            AchivementStatisticsStorage storage = new AchivementStatisticsStorage(new TableStorageConnection());
            await storage.GenerateToAdjustAchivementBody();
            return Json(true);
        }
    }
}
