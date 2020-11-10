using smallWorld.ViewModel;
using System;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Runtime.CompilerServices;
using smallWorld.Services;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace smallWorld.Controllers
{
    public class HomeController : Controller
    {
        private readonly MemberService memberservice = new MemberService();
        private readonly MailService mailservice = new MailService();
        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            //dbCustomerEntities db = new dbCustomerEntities();
            //CData c = new CData();
            //var x = from i in db.Member
            //        where i.fMemberId == id
            //        select i;
            //var y = from j in db.UserRole
            //        where j.fUserId == id
            //        select j;
            //c.memberData = x;
            //c.userData = y;
            //return View(c);
            return View();
        }

        #region 註冊

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude = "buildTime,authCode,Role")]CRegister member)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (dbCustomerEntities db = new dbCustomerEntities())
                    {
                        //當用戶已存在
                        if (db.Member.Where(a => a.fAccount == member.account).FirstOrDefault() != null)
                        {
                            //設定模型驗證欄位狀態失敗顯示訊息
                            ModelState.AddModelError("account","您註冊的帳號已經被使用，請重新設定");
                            //回傳模型檢視結果
                            return View(member);
                        }
                        //宣告與建構交易式物件操作案例並自動釋放占用資源 => 確保資料可以寫入資料庫且必須完成
                        using (TransactionScope ts = new TransactionScope())
                        {
                            memberservice.Register(member);
                            //寄信
                            ////取得信箱驗證碼
                            //string AuthCode = mailservice.getValidationCode();
                            ////填入驗證碼
                            //member.authCode = AuthCode;
                            ////取得驗證信範本
                            //string tempmail = System.IO.File.ReadAllText(
                            //    Server.MapPath("~/Views/Shared/registerEmailTemplate.html"));
                            ////宣告email驗證用的url
                            //UriBuilder vUri = new UriBuilder(Request.Url)
                            //{
                            //    Path = Url.Action("emailValidate", "Home", new
                            //    {
                            //        account = member.account,
                            //        authcode = AuthCode
                            //    })
                            //};
                            ////填入驗證信
                            //string mailBody = mailservice.getRegisterMailBody(tempmail, member.name, vUri.ToString().Replace("%3F", "?"));
                            ////寄信
                            //mailservice.sendRegisterMail(mailBody, member.email);
                            ////用tempData儲存註冊訊息
                            ///TempData["RegisterState"] = "註冊成功，請去收信以驗證email";
                            TempData["RegisterState"] = "註冊成功，請重新登入";
                            ////設定所有交易皆已完成
                            ts.Complete();
                            return RedirectToAction("registerResult","Home");
                        }
                    }
                }
                catch(SmtpException)
                {
                    ModelState.AddModelError("email", "系統發生異常，目前無法寄送驗證信，請稍後再試");

                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                return RedirectToAction("Login");
            }
            else{
                //未經驗證清空密碼相關欄位
                member.password = null;
                member.password_confirm = null;
                return View(member);
            }
        }
        //註冊驗證
        public ActionResult Verify(string AuthCode)
        {
            using (dbCustomerEntities db = new dbCustomerEntities())
            {
                var memberData = db.Member.Where(a => a.fAuthCode == AuthCode).FirstOrDefault();
                if (memberData != null)
                {
                    ViewData["Result"] = "會員驗證成功";
                    memberData.fAuthCode = null;
                    db.SaveChanges();
                }
                else {
                    ViewData["Result"] = "找不到此驗證碼，請確認是否驗證過?";
                }
            }
                return View();
        }
        //註冊結果顯示
        public ActionResult registerResult()
        {
            return View();
        }
        //接收驗證信連結傳進來
        public ActionResult emailValidation(string account, string AuthCode)
        {
            //用ViewData儲存，使用Service進行信箱驗證後的結果訊息
            ViewData["emailValidate"] = memberservice.emailValidation(account, AuthCode);
            return View();
        }
        

        #endregion
        #region 登入
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(CLogin c)
        {
            if (c.account == "admin" && c.password == "admin")
                return RedirectToAction("SearchAccountRole", "Account");
            //用戶登入通過驗證
            if (ValidateLogin(c.account, c.password))
            {
                //執行將用戶登入到網站並授予存取權
                LoginProcess(c.account, strRole, false);
                return RedirectToAction("Index","Home");
            }
            else {
                ModelState.AddModelError("validatemsg_fAccount", "輸入的帳號或密碼錯誤，請重來");
            }
            return View();
        }
        //宣告私有字串
        private string strRole = "User";
        //用戶登入存取權限
        private void LoginProcess(string strUser,string strRole,bool isRemember)
        {
            //登入前清空所有session資料
            Session.RemoveAll();
            //表單票證通行
            FormsAuthenticationTicket objfat = new FormsAuthenticationTicket(
                1,//版本    
                strUser,//欲存放在User.Identy.Name值,通常是使用者帳號
                DateTime.Now,//核發日期
                DateTime.Now.AddMinutes(30),//到期日期(30分鐘)
                false,//將管理者登入的cookie設定成session cookie(持續性)
                strRole,//userData(使用者專屬資料)
                FormsAuthentication.FormsCookiePath//cookie路徑
                );
            //建立包含適用於http_cookie加密的表單驗證票證字串
            string strEncryptTicket = FormsAuthentication.Encrypt(objfat);
            //將加密的票證字串存入cookie
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName,strEncryptTicket));
        }
        //用戶登入驗證
        private bool ValidateLogin(string fAccount,string fPassword)
        {
            //用戶密碼加密後處理
            //string strHashPassword = HashPassword(fPassword);
            string strHashPassword = fPassword;

            using (dbCustomerEntities db = new dbCustomerEntities())
            {
                //取得符合條件單筆資料
                Member member = db.Member.Where(a => a.fAccount == fAccount && a.fPassword == fPassword).FirstOrDefault();
                if (member != null)
                {
                    //會員未點擊驗證碼連結
                    if (member.fAuthCode != null)
                    {
                        ModelState.AddModelError("account", "信箱尚未驗證成功");
                        return false;
                    }
                    //若用戶為管理者
                    if (member.fRole == 3)
                    {
                        strRole = "Admin";
                    }
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        #endregion
        #region 登出
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        #endregion
    }
}