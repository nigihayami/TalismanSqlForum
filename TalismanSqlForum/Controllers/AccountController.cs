using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TalismanSqlForum.Models;
using System.Net.Mail;

namespace TalismanSqlForum.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;

        private ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var t = new LoginViewModel();
            ViewBag.ReturnUrl = returnUrl;
            return View(t);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await UserManager.FindAsync(model.Email, model.Password);
            if (user != null)
            {
                await SignInAsync(user, model.RememberMe);
                using (var db = new ApplicationDbContext())
                {
                    var t = db.Users.First(a => a.Email == model.Email);
                    if (t != null)
                    {
                        t.LastIn = DateTime.Now;
                        db.Entry(t).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    db.Dispose();
                }
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError("", "Недопустимое имя пользователя или пароль.");
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                Adres = model.Adres,
                Contact_Name = model.Contact_Name,
                Inn = model.Inn,
                Mnemo_Org = model.Mnemo_Org,
                Name_Org = model.Name_Org,
                NickName = model.NickName,
                PhoneNumber = model.PhoneNumber,
                LastIn = DateTime.Now,
                DateReg = DateTime.Now,
                IsNew = true
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await SignInAsync(user, isPersistent: false);
                var mail = new MailMessage
                {
                    Subject = "Новый пользователь",
                    Body = "<p>Модератор!!! В системе появился новый пользователь " + model.Email + "</p>",
                    IsBodyHtml = true
                };

                using (var db = new ApplicationDbContext())
                {
                    foreach (var item2 in db.Roles.Where(a => a.Name == "moderator").SelectMany(item => item.Users))
                    {
                        mail.To.Add(db.Users.Find(item2.UserId).Email);
                    }
                    db.Dispose();
                }
                Code.Notify.NewUser(user.Id);
                return RedirectToAction("Index", "ForumList");
            }
            //Есть такие люди
            using (var db = new ApplicationDbContext())
            {
                AddErrors(db.Users.Any(a => a.Email == model.Email)
                    ? new IdentityResult(new string[] { "Данный Email уже используется" })
                    : result);
            }
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                await SignInAsync(user, isPersistent: false);
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ваш пароль изменен."
                : message == ManageMessageId.SetPasswordSuccess ? "Пароль задан."
                : message == ManageMessageId.RemoveLoginSuccess ? "Внешнее имя входа удалено."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            var hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (!ModelState.IsValid) return View(model);
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
            }
            else
            {
                // Для пользователя не указан пароль, поэтому все ошибки проверки из-за отсутствия поля OldPassword будут удалены
                var state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (!ModelState.IsValid) return View(model);
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }


        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Запрос перенаправления к внешнему поставщику входа для связывания имени входа для текущего пользователя
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "ForumList");
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        public ActionResult ManageUser()
        {
            ViewData["userId"] = User.Identity.GetUserId();
            using (var db = new ApplicationDbContext())
            {
                var u = db.Users.Find(ViewData["userId"]);
                var t = new ManageUserView
                {
                    Adres = u.Adres,
                    Inn = u.Inn,
                    Mnemo_Org = u.Mnemo_Org,
                    Name_Org = u.Name_Org,
                    NickName = u.NickName,
                    PhoneNumber = u.PhoneNumber,
                    Email = u.Email
                };
                db.Dispose();
                return View(t);
            }
        }
        [HttpPost]
        public ActionResult ManageUser(string id, ManageUserView model)
        {
            using (var db = new ApplicationDbContext())
            {
                var t = db.Users.Find(id);
                if (t == null)
                    return HttpNotFound();
                if (t.Email != model.Email)
                {
                    if (db.Users.Any(a => a.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "Данный Email уже используется");
                    }
                    else
                    {
                        t.Email = model.Email;
                        t.UserName = model.Email;
                    }
                }
                t.Adres = model.Adres;
                t.Inn = model.Inn;
                t.Mnemo_Org = model.Mnemo_Org;
                t.Name_Org = model.Name_Org;
                t.NickName = model.NickName;
                t.PhoneNumber = model.PhoneNumber;
                if (ModelState.IsValid)
                {
                    db.Entry(t).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    db.Dispose();
                    return RedirectToAction("Index", "ForumList");
                }
                db.Dispose();
                return View(model);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private void SendEmail(string email, string callbackUrl, string subject, string message)
        {
            // Дополнительные сведения об отправке электронной почты см. по адресу: http://go.microsoft.com/fwlink/?LinkID=320771
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "ForumList");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            private string LoginProvider { get; set; }
            private string RedirectUri { get; set; }
            private string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}