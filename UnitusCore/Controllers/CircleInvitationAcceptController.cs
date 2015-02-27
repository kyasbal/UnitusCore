using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class CircleInvitationAcceptController : UnitusController
    {
        private const string ServerSecret = "#Snisr1Zrd01VLfnl1c&Qx-buRikCn8CWH8RHcM-4NquKvL3m0";

        internal CircleMemberInvitation getInvitationData(string confirmationId)
        {
            return
                DbSession.CircleInvitations.Where(a => a.ConfirmationKey.Equals(confirmationId))
                    .Include(a => a.InvitedCircle)
                    .Include(a => a.InvitedPerson)
                    .FirstOrDefault();
        }

        internal static string createSecurityHash(string mailAddr, string confirmationId)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bs = md5.ComputeHash(Encoding.Unicode.GetBytes(mailAddr + confirmationId + ServerSecret));
            md5.Clear();
            var result = new StringBuilder();
            foreach (var b in bs)
            {
                result.Append(b.ToString("x2"));
            }
            return result.ToString();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Index(string confirmationId)
        {
            var invitationData = getInvitationData(confirmationId);
            if (invitationData != null)
            {
                return
                    View(new CircleInvitationAcceptResponse(invitationData.InvitedPerson.Name,
                        invitationData.InvitedCircle.Name, confirmationId, invitationData.EmailAddress,invitationData.InvitedCircle.BelongedSchool));
            }
            return Json(false);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmailSelect(CircleInvitationEmailSelectRequest req)
        {
            var invitationData = getInvitationData(req.ConfirmationId);
            var email = req.UseSentEmail ? invitationData.EmailAddress : req.NewEmail;
            var user = UserManager.FindByEmail(email);
            if (user == null)
            {
                return View("SelectedEmailForUnknownUser",
                    new CircleInvitationAcceptResponse(invitationData.InvitedPerson.Name,
                        invitationData.InvitedCircle.Name, req.ConfirmationId, email,invitationData.InvitedCircle.BelongedSchool));
            }
            //userがnullでない、すでに登録されているユーザーの場合。
            return View("SelectedEmailForAlreadyUser",
                new CircleInvitationSelectMailForAlreadyExisiting(invitationData.InvitedPerson.Name,
                    invitationData.InvitedCircle.Name, req.ConfirmationId, email,
                    createSecurityHash(user.Email, invitationData.ConfirmationKey)));
        }

        private void RemoveInvitation(CircleMemberInvitation removeTarget)
        {
            DbSession.CircleInvitations.Remove(removeTarget);
            DbSession.SaveChanges();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmailSelectConfirmForUnknownUser(string confirmationId,
            [EmailAddress] string username, [Required]string firstName, [Required]string lastName, string password,string NewBelongedUniversity)
        {
            if (ModelState.IsValid)
            {
                var invitationData = getInvitationData(confirmationId);
                var state = UserManager.CreateUser(DbSession, username, password);
                if (state.Succeeded)
                {
                    DbSession.SaveChanges();
                    var user = UserManager.FindByName(username);
                    await user.LoadPersonData(DbSession);
                    await user.RetrieveBelongingCircles(DbSession);
                    user.PersonData.Name = firstName + " " + lastName;
                    string belongedTo = string.IsNullOrWhiteSpace(NewBelongedUniversity)
                        ? invitationData.InvitedCircle.BelongedSchool
                        : NewBelongedUniversity;
                    user.PersonData.BelongedColledge = belongedTo;
                    var status = new MemberStatus();
                    status.GenerateId();
                    status.Occupation = "新入生";
                    status.IsActiveMember = true;
                    status.TargetUser = user.PersonData;
                    status.TargetCircle = invitationData.InvitedCircle;
                    user.PersonData.BelongedCircles.Add(status);
                    DbSession.MemberStatuses.Add(status);
                    DbSession.SaveChanges();
                    if (invitationData.EmailAddress.Equals(username)) //この時メールの確認は必要ない。
                    {
                        RemoveInvitation(invitationData);
                        return View("InviteAcceptMessage",
                            new SimpleMessage(string.Format("アカウント:{0}は正常に作成されました。", user.UserName), false));
                    }
                    var result = MailConfirmationManager.SendMailConfirmation(user, this);
                    if (result.Success)
                    {
                        RemoveInvitation(invitationData);
                        return View("InviteAcceptMessage",
                            new SimpleMessage(
                                string.Format("アカウント:{0}は正常に作成されました。{0}にアカウントの確認のためのメールを送りました。", user.UserName),
                                false));
                    }
                    RemoveInvitation(invitationData);
                    return View("InviteAcceptMessage", new SimpleMessage(string.Format("アカウント:{0}は正常に作成されました。" +
                                                                                       "", user.UserName), false));
                }
                return View("SelectedEmailForUnknownUser",
                    new CircleInvitationAcceptResponse(invitationData.InvitedPerson.Name,
                        invitationData.InvitedCircle.Name, confirmationId, username,invitationData.InvitedCircle.BelongedSchool)
                    {
                        ErrorMessage = state.Errors.FirstOrDefault()
                    });
            }
            return View("InviteAcceptMessage", new SimpleMessage("無効なデータが渡されました。", true));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> EmailSelectConfirmForAlreadyExisiting(string confirmationId, string securityHash,
            string mailaddr)
        {
            if (createSecurityHash(CurrentUser.Email, confirmationId).Equals(securityHash))
            {
//OKのとき
                var invitationData= getInvitationData(confirmationId);
                await invitationData.LoadReferences(DbSession);
                DbSession.CircleInvitations.Remove(invitationData);
                var isAlreadyMember = false;
                foreach (var member in CurrentUser.PersonData.BelongedCircles)
                {
                    await member.LoadReferencesAsync(DbSession);
                    if (member.TargetCircle.Id.Equals(invitationData.InvitedCircle.Id))
                    {
                        isAlreadyMember = true;
                        break;
                    }
                }
                if (isAlreadyMember)
                {
                    RemoveInvitation(invitationData);
                    DbSession.SaveChanges();
                    this.AddNotification(NotificationType.Error, "招待の受諾(エラー)",
                        string.Format("「{0}」に既にあなたは参加しています。", invitationData.InvitedCircle.Name));
                }
                else
                {
                    var status = new MemberStatus();
                    status.GenerateId();
                    status.Occupation = "新入生";
                    status.IsActiveMember = true;
                    status.TargetCircle = invitationData.InvitedCircle;
                    status.TargetUser = CurrentUser.PersonData;
                    CurrentUser.PersonData.BelongedCircles.Add(status);
                    DbSession.SaveChanges();
                    RemoveInvitation(invitationData);
                    this.AddNotification(NotificationType.Success, "招待の受諾",
                        string.Format("「{0}」に参加しました。", invitationData.InvitedCircle.Name));
                }
                return RedirectToAction("Index", "Home");
            }
            //無効なとき
            if (mailaddr.Equals(User.Identity.Name))
            {
                return View("InviteAcceptMessage", new SimpleMessage("無効なログインです。", false));
            }
            return View("InviteAcceptMessage",
                new SimpleMessage(string.Format("無効なログインです。この招待を受諾するには、「{0}」でログインする必要があります。", mailaddr), false));
        }
    }

    public class SimpleMessage
    {
        public SimpleMessage(string errorMessage, bool isError)
        {
            ErrorMessage = errorMessage;
            IsError = isError;
        }

        public string ErrorMessage { get; set; }

        public bool IsError { get; set; }
    }

    public class CircleInvitationEmailSelectRequest
    {
        public string ConfirmationId { get; set; }

        public bool UseSentEmail { get; set; }

        public string NewEmail { get; set; }
    }

    public class CircleInvitationAcceptResponse
    {
        public CircleInvitationAcceptResponse(string inviterName, string circleName, string confirmationId,
            string emailAddress,string belongedUniveristy)
        {
            InviterName = inviterName;
            CircleName = circleName;
            ConfirmationId = confirmationId;
            EmailAddress = emailAddress;
            BelongedUniversity = belongedUniveristy;
        }

        public string InviterName { get; set; }

        public string CircleName { get; set; }

        public string ConfirmationId { get; set; }

        public string EmailAddress { get; set; }

        public string ErrorMessage { get; set; }

        public string BelongedUniversity { get; set; }
    }

    public class CircleInvitationSelectMailForAlreadyExisiting
    {
        public CircleInvitationSelectMailForAlreadyExisiting(string inviterName, string circleName,
            string confirmationId, string emailAddress, string confirmationHash)
        {
            InviterName = inviterName;
            CircleName = circleName;
            ConfirmationId = confirmationId;
            EmailAddress = emailAddress;
            ConfirmationHash = confirmationHash;
        }

        public string InviterName { get; set; }

        public string CircleName { get; set; }

        public string ConfirmationId { get; set; }

        public string EmailAddress { get; set; }

        public string ConfirmationHash { get; set; }
    }
}