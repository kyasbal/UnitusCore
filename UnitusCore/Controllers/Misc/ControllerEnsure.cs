﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using UnitusCore.Controllers.Base;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels.Achivement;

namespace UnitusCore.Controllers.Misc
{
    public class ControllerEnsure
    {
        private readonly IUnitusController _controller;

        public ControllerEnsure(IUnitusController controller)
        {
            _controller = controller;
        }

        private AchivementStatisticsStorage _achivementStorage;
        public AchivementStatisticsStorage AchivementStorage
        {
            get
            {
                _achivementStorage = _achivementStorage ??
                                     new AchivementStatisticsStorage(new TableStorageConnection(), _controller.DbSession);
                return _achivementStorage;
            }
        }

        public ApplicationUser ExistingUserFromId(string userId)
        {
            ApplicationUser user = _controller.DbSession.Users.Find(userId);
            if (user == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return user;
        }

        public async Task<ApplicationUser> ExistingUserFromEmail(string email)
        {
            ApplicationUser user = await _controller.UserManager.FindByNameAsync(email);
            if (user == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return user;
        }

        public async Task<Circle> ExisitingCircleId(string circleId)
        {
            Guid circleGuid = ValidGuid(circleId);
            Circle circle = await _controller.DbSession.Circles.FindAsync(circleGuid);
            if (circle == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return circle;
        }

        public async Task<Circle> BelongingTo(string circleId, string userId)
        {
            Circle circle = await ExisitingCircleId(circleId);
            await circle.LoadMembers(_controller.DbSession,true,true);
            if (!await circle.IsMember(_controller.DbSession, userId))throw new HttpResponseException(HttpStatusCode.BadRequest);
            return circle;
        }

        public async Task<AchivementBody> ExistingAchivementName(string achivementName)
        {
            AchivementBody achivementBody = await AchivementStorage.RetrieveAchivementBody(achivementName);
            if(achivementBody==null)throw new HttpResponseException(HttpStatusCode.NotFound);
            return achivementBody;
        }

        public async Task<Person> ExistingPersonId(string personId)
        {
            Guid personGuid = ValidGuid(personId);
            Person person = await _controller.DbSession.People.FindAsync(personGuid);
            if(person==null)throw new HttpResponseException(HttpStatusCode.NotFound);
            return person;
        }

        public Guid ValidGuid(string guid)
        {
            Guid guidResult;
            if(!Guid.TryParse(guid,out guidResult))throw new HttpResponseException(HttpStatusCode.BadRequest);
            return guidResult;
        }
    }
}