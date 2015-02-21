using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;

namespace UnitusCore.Storage
{
    public class DbCacheStorage
    {
        private const string CircleMemberCacheTableName = "CircleMemberCache";

        private readonly TableStorageConnection _connection;
        private readonly ApplicationDbContext _dbSession;

        private readonly CloudTable _circleMemberCacheTable;

        public DbCacheStorage(TableStorageConnection connection,ApplicationDbContext dbSession)
        {
            _connection = connection;
            _dbSession = dbSession;
            _circleMemberCacheTable = connection.TableClient.GetTableReference(CircleMemberCacheTableName);
            _circleMemberCacheTable.CreateIfNotExists();
        }

        public async Task UpdateAllCircles(IEnumerable<Circle> circles)
        {
            foreach (var circle in circles)
            {
                await UpdateCircle(circle);
            }
        }

        public async Task<HashSet<string>> GetCircleMemberGitLogins(string userId)
        {
            var user=_dbSession.Users.Find(userId);
            await user.LoadPersonData(_dbSession);
            await user.PersonData.LoadBelongingCircles(_dbSession);
            IdPairContainerStorage pairStroage=new IdPairContainerStorage(_connection);
            HashSet<string> joiningCircleIds=new HashSet<string>();
            HashSet<string> githubLogins=new HashSet<string>();
            foreach (var circleStatus in user.PersonData.BelongedCircles)
            {
                await circleStatus.LoadReferencesAsync(_dbSession);
                joiningCircleIds.Add(circleStatus.TargetCircle.Id.ToString());
            }
            foreach (string circleId in joiningCircleIds)
            {
                await EachMember(circleId, async q =>
                {
                    string id=await pairStroage.RetrieveId(q.UserId, IdPairContainer.UserId, IdPairContainer.GithubLogin);
                    githubLogins.Add(id);
                });
            }
            return githubLogins;
        }

        private async Task EachMember(string circleId, Func<CircleMemberCache,Task> f)
        {
            TableQuery<CircleMemberCache> query =
                new TableQuery<CircleMemberCache>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, circleId));
            foreach (CircleMemberCache member in _circleMemberCacheTable.ExecuteQuery(query))
            {
                await f(member);
            }
        }

        private async Task UpdateCircle(Circle circle)
        {
            string circleId = circle.Id.ToString();
            TableQuery<CircleMemberCache> query = new TableQuery<CircleMemberCache>().Where(TableQuery.GenerateFilterCondition("PartitionKey",QueryComparisons.Equal,circleId));
            foreach (CircleMemberCache member in _circleMemberCacheTable.ExecuteQuery(query))
            {
                await _circleMemberCacheTable.ExecuteAsync(TableOperation.Delete(member));
            }
            await circle.LoadMembers(_dbSession);
            foreach (MemberStatus member in circle.Members)
            {
                await member.LoadReferencesAsync(_dbSession);
                await member.TargetUser.LoadApplicationUser(_dbSession);
                await _circleMemberCacheTable.ExecuteAsync(
                    TableOperation.InsertOrReplace(new CircleMemberCache(circleId, member.TargetUser.ApplicationUser.Id,
                        member.TargetUser.Id.ToString())));
            }
        }

        
    }
}