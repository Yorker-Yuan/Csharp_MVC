using smallWorld.Models;
using smallWorld.ViewModel;
using System;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace smallWorld.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        #region 註冊

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register([Bind(Exclude = "fBuildtime,fAuthCode,fRole")]Member member)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (dbCustomerEntities db = new dbCustomerEntities())
                    {
                        //當用戶已存在
                        if (db.Member.Where(a => a.fMemberId == member.fMemberId).FirstOrDefault() != null)
                        {
                            //設定模型驗證欄位狀態失敗顯示訊息
                            ModelState.AddModelError("fMemberId","您註冊的帳號已經被使用，請重新設定");
                            //回傳模型檢視結果
                            return View(member);
                        }
                        //宣告與建構交易式物件操作案例並自動釋放占用資源 => 確保資料可以寫入資料庫且必須完成
                        using (TransactionScope ts = new TransactionScope())
                        {
                            member.fRole = 0;
                            member.fAuthCode = Guid.NewGuid().ToString();
                            //member.fPassword = HashPassword(member.fPassword);
                            member.fPassword = member.fPassword;
                            db.Member.Add(member);
                            db.SaveChanges();
                            //寄信

                            //設定所有交易皆已完成
                            ts.Complete();
                        }
                    }

                }
                catch(SmtpException)
                {
                    ModelState.AddModelError("fEmail","系統發生異常，目前無法寄送驗證信，請稍後再試");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("fMemberId", e.Message.ToString());
                    return View(member);
                }
                return RedirectToAction("Login");
            }
            else{
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
        //加密
        private string HashPassword(string str)
        {
            string strHash = "";
            //字串雜湊編碼
            System.Security.Cryptography.SHA1 objsha = System.Security.Cryptography.SHA1.Create();
            System.Text.ASCIIEncoding objasc = new System.Text.ASCIIEncoding();
            byte[] byteCom = objasc.GetBytes(str);
            objsha.ComputeHash(byteCom);
            strHash = Convert.ToBase64String(objsha.Hash);
            return strHash;
        }

        #endregion
#region 登入
        public ActionResult Login()
        {
            return View();
        }
#endregion
        public ActionResult Logout()
        {
            return View();
        }
    }
}