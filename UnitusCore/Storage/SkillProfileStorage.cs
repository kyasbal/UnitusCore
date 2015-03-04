using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels.Profile;

namespace UnitusCore.Storage
{
    public class SkillProfileStorage : TableStorageBase
    {
        private static readonly string SkillInfoTableName = "SkillInfomation";
        private static readonly string SkillProfileTableName = "SkillProfiles";
        private readonly CloudTable _skillInfo;
        private readonly CloudTable _skillProfile;

        public SkillProfileStorage(TableStorageConnection storageConnection) : base(storageConnection)
        {
            _skillInfo = InitCloudTable(SkillInfoTableName);
            _skillProfile = InitCloudTable(SkillProfileTableName);

        }

        public IEnumerable<string> GetAllSkills()
        {
            var infoQuery =
                new TableQuery<SkillInfo>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, "SKILL"));
            return _skillInfo.ExecuteQuery(infoQuery).Select(a => a.SkillName);
        }

        public async Task AppendSkill(string skillName)
        {
            await _skillInfo.ExecuteAsync(TableOperation.InsertOrReplace(new SkillInfo(skillName)));
        }

        public IEnumerable<SkillProfile> GetAllSkillProfile(string userId)
        {
            var profileQuery =
                new TableQuery<SkillProfile>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, userId));
            return _skillProfile.ExecuteQuery(profileQuery);
        }

        public async Task AddOrReplaceProfile(string userId, string skillname, SkillLevel level)
        {
            await _skillProfile.ExecuteAsync(TableOperation.InsertOrReplace(new SkillProfile(userId, skillname, level)));
        }
    }
}