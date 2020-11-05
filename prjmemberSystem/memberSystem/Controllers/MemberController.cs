using memberSystem.Services;
using memberSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace memberSystem.Controllers
{
    public class MemberController : Controller
    {
        private readonly MemberDBservice memberservice = new MemberDBservice();
        private readonly MailService mailservice = new MailService();
        // GET: Member
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult register() {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Home", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult register(CRegister c)
        {
            //判斷頁面是否通過驗證
            if (ModelState.IsValid) {
                //將頁面的密碼填入
                c.newMember.password = c.password;
                //取得信箱驗證碼
                string authcode = mailservice.getValidateCode();
                //填入
                c.newMember.authCode = authcode;
                //呼叫service註冊新會員
                memberservice.register(c.newMember);
                //取得驗證信範本
                string tempmail = System.IO.File.ReadAllText(
                    Server.MapPath("~/Views/Shared/registerEmailTemplate.html"));
                //宣告email驗證用的url
                UriBuilder vUri = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("emailValidate","Member",new { 
                        account = c.newMember.account,
                        authcode = authcode
                    })
                };
                //填入驗證信
                string mailBody = mailservice.getRegisterMailBody(tempmail,c.newMember.name,vUri.ToString().Replace("%3F","?"));
                //寄信
                mailservice.sendRegisterMail(mailBody,c.newMember.email);
                //用tempData儲存註冊訊息
                TempData["RegisterState"] = "註冊成功，請去收信以驗證email";
                return RedirectToAction("registerResult");
            }
            //未經驗證清空密碼相關欄位
            c.password = null;
            c.password_confirm = null;
            return View(c);
        }
        //註冊結果顯示
        public ActionResult registerResult()
        {
            return View();
        }
        //判斷註冊帳號是否已註冊
        public JsonResult accountCheck(CRegister c)
        {
            return Json(memberservice.accountCheck(c.newMember.account),JsonRequestBehavior.AllowGet);
        }
        //接收驗證信連結傳進來
        public ActionResult emailValidation(string account, string AuthCode)
        {
            //用ViewData儲存，使用Service進行信箱驗證後的結果訊息
            ViewData["emailValidate"] = memberservice.emailValidate(account,AuthCode);
            return View();
        }

    }
}