using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    /// <summary>
    /// コントローラーの入力引数の検証クラス
    /// </summary>
    public class ControllerEnsure
    {
        private readonly IUnitusController _controller;

        public ControllerEnsure(IUnitusController controller)
        {
            _controller = controller;
        }

        private AchivementStatisticsStorage _achivementStorage;

        private AchivementStatisticsStorage AchivementStorage
        {
            get
            {
                _achivementStorage = _achivementStorage ??
                                     new AchivementStatisticsStorage(new TableStorageConnection(), _controller.DbSession);
                return _achivementStorage;
            }
        }

        /// <summary>
        /// 指定されたIDが存在するユーザーか調べ、違う場合は404NotFoundをスローします
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザーが存在した場合はユーザーアカウント</returns>
        public ApplicationUser ExistingUserFromId(string userId)
        {
            ApplicationUser user = _controller.DbSession.Users.Find(userId);
            if (user == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return user;
        }

        /// <summary>
        /// 指定されたメアドが存在するか調べ、存在しない場合は404NotFoundをスローします。
        /// </summary>
        /// <param name="email">ユーザーのメールアドレス</param>
        /// <returns>ユーザーが存在した場合はユーザーアカウント</returns>
        public async Task<ApplicationUser> ExistingUserFromEmailAsync(string email)
        {
            ApplicationUser user = await _controller.UserManager.FindByNameAsync(email);
            if (user == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return user;
        }
        /// <summary>
        /// 指定したIDのサークルが存在するか調べ、存在しない場合は404NotFoundをスローします。
        /// circleIdがGUIDの形式を満たしていない場合は400BadRequestになります。
        /// </summary>
        /// <param name="circleId">サークルのID</param>
        /// <returns>サークルが存在した場合はサークルデータ</returns>
        public async Task<Circle> ExisitingCircleIdAsync(string circleId)
        {
            Guid circleGuid = ValidGuid(circleId);
            Circle circle = await _controller.DbSession.Circles.FindAsync(circleGuid);
            if (circle == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return circle;
        }
        /// <summary>
        /// 指定したユーザーIDが指定したサークルに所属しているか調べ、所属している場合はサークルを返します。
        /// このAPIはサークルをとってくるために利用するべきではありません。
        /// 特定のサーバーAPIにおいて、特定のサークルへの所属が前提である際にその要求が適切であるかチェックします。
        /// 所属していない場合は400BadRequestをスローします。
        /// </summary>
        /// <param name="circleId">サークルID</param>
        /// <param name="userId">ユーザID</param>
        /// <returns>取得できたサークル</returns>
        public async Task<Circle> BelongingToAsync(string circleId, string userId)
        {
            Circle circle = await ExisitingCircleIdAsync(circleId);
            await circle.LoadMembers(_controller.DbSession,true,true);
            if (!await circle.IsMember(_controller.DbSession, userId))throw new HttpResponseException(HttpStatusCode.BadRequest);
            return circle;
        }
        /// <summary>
        /// 指定した実績名が存在するものであるか検証します。存在する実績名だった場合対応する実績データを返します。
        /// 存在しなかった場合は404NotFoundをスローします。
        /// </summary>
        /// <param name="achivementName">実績名</param>
        /// <returns>実績データ</returns>
        public async Task<AchivementBody> ExistingAchivementName(string achivementName)
        {
            AchivementBody achivementBody = await AchivementStorage.RetrieveAchivementBody(achivementName);
            if(achivementBody==null)throw new HttpResponseException(HttpStatusCode.NotFound);
            return achivementBody;
        }

        /// <summary>
        /// 指定したPersonIdが存在するか検証します。PersonIdは可能な限りUserIdを利用した検証を行うべきです。
        /// 存在しなかった場合404NotFoundをスローします。指定したIDが正常な形式でない場合は400BadRequestをスローします。
        /// </summary>
        /// <param name="personId">PersonID</param>
        /// <returns>取得されたPersonData(プロフィール情報)</returns>
        public async Task<Person> ExistingPersonIdAsync(string personId)
        {
            Guid personGuid = ValidGuid(personId);
            Person person = await _controller.DbSession.People.FindAsync(personGuid);
            if(person==null)throw new HttpResponseException(HttpStatusCode.NotFound);
            return person;
        }
        /// <summary>
        /// 正常なGUIDかのチェック
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private Guid ValidGuid(string guid)
        {
            Guid guidResult;
            if(!Guid.TryParse(guid,out guidResult))throw new HttpResponseException(HttpStatusCode.BadRequest);
            return guidResult;
        }
        /// <summary>
        /// 指定したIDがNULLや空文字でないことを検証します。
        /// NULLだった場合特定のメッセージと共にBadRequestをスローします。
        /// </summary>
        /// <param name="str">検証するテキスト</param>
        /// <param name="info">空もしくはnullのときに表示されるエラーメッセージ</param>
        public void NotEmptyString(string str,string info="")
        {
            if(string.IsNullOrWhiteSpace(str))throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(info)
            });
        }
        /// <summary>
        /// 渡した列挙体の中に含まれているのか検証します。
        /// </summary>
        /// <typeparam name="T">検証したい対象の列挙体</typeparam>
        /// <param name="arg">検証文字列</param>
        /// <returns>存在した場合は列挙体要素</returns>
        public T IsEnumElement<T>(string arg)where T:struct 
        {
            T t;
            if (!Enum.TryParse(arg, out t))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = typeof(T).ToString()+"is not contain"+arg
                });
            }
            return t;
        }

        /// <summary>
        /// 指定したものがスキル名かどうか検証します。
        /// ない場合はBadRequest
        /// </summary>
        /// <param name="skillName">検証するスキル名</param>
        /// <returns></returns>
        public async Task IsSkillName(string skillName)
        {
            SkillProfileStorage sps=new SkillProfileStorage(new TableStorageConnection());
            if (!await sps.IsSkill(skillName)) throw new HttpResponseException(HttpStatusCode.BadRequest);
        }
        /// <summary>
        /// 指定したスキル名が存在しない物であることを決定づけます
        /// 存在する場合はConflictをスローします。
        /// </summary>
        /// <param name="skillName">検証するスキル名</param>
        /// <returns></returns>
        public async Task IsNotExisitingSkillName(string skillName)
        {
            NotEmptyString(skillName);
            SkillProfileStorage sps = new SkillProfileStorage(new TableStorageConnection());
            if (await sps.IsSkill(skillName)) throw new HttpResponseException(HttpStatusCode.Conflict);
        }
    }
}