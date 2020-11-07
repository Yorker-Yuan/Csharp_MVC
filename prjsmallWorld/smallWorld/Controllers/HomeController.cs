﻿using smallWorld.Models;
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

namespace smallWorld.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        []
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
        [HttpPost]
        public ActionResult Login(string fAccount,string fPassword)
        {
            //用戶登入通過驗證
            if (ValidateLogin(fAccount, fPassword))
            {
                //執行將用戶登入到網站並授予存取權
                LoginProcess(fAccount, strRole, false);
                return RedirectToAction("Index");
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
                        ModelState.AddModelError("fAccount", "信箱尚未驗證成功");
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