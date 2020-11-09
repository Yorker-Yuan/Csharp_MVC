using smallWorld.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace smallWorld.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        dbCustomerEntities db = new dbCustomerEntities();
        // GET: Account
        [Authorize(Roles = "User, VIPUser")]
        public ActionResult Index()
        {
            var user = from a in db.Member
                       select a;
            return View(user);
        }
        //修改密碼
        [Authorize]
        public ActionResult ResetPassword(int id)
        {
            //using (dbJoutaEntities db = new dbJoutaEntities())
            //{
            //    var user = db.tMember.Where(a => a.f重置驗證碼 == id).FirstOrDefault();
            //    if (user != null)
            //    {
            //        CReset c = new CReset();
            //        c.resetCode = id;
            //        return View(c);
            //    }
            //    else
            //    {
            //        return HttpNotFound();
            //    }
            //}
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ResetPassword()
        {
            //var message = "";
            //if (ModelState.IsValid)
            //{
            //    using (dbJoutaEntities db = new dbJoutaEntities())
            //    {
            //        var user = db.tMember.Where(a => a.f重置驗證碼 == c.resetCode).FirstOrDefault();
            //        if (user != null)
            //        {
            //            user.f會員密碼 = c.newPassword;
            //            user.f重置驗證碼 = "";
            //            db.Configuration.ValidateOnSaveEnabled = false;
            //            db.SaveChanges();
            //            message = "新密碼重置成功!";
            //        }
            //    }
            //}
            //else
            //{
            //    if (c.newPassword == null)
            //    {
            //        message = "內容必填";
            //    }
            //    else
            //        message = "格式錯誤";
            //}
            //ViewBag.Message = message;
            return View();
        }

        //修改
        [Authorize(Roles ="VIPUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Member.FirstOrDefault(a =>a.fMemberId == id);
            return View(member);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "VIPUser")]
        public ActionResult Edit([Bind(Exclude = "fBuildTime,fAuthCode,fRole")]Member member)
        {
            Member m = db.Member.FirstOrDefault(a=>a.fMemberId == member.fMemberId);
            if (m != null)
            {
                m.fAccount = member.fAccount;
                m.fPassword = member.fPassword;
                m.fName = member.fName;
                m.fBirthday = member.fBirthday;
                m.fEmail = member.fEmail;
            }
            return RedirectToAction("Index");
        }
        //瀏覽帳號權限
        [Authorize(Roles = "Admin")]
        public ActionResult SearchAccountRole(int? id)
        {
            return View();
        }
        //編輯帳號權限
        [Authorize(Roles = "Admin")]
        public ActionResult EditAccountRole(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Member.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccountRole(UserRole userrole)
        {
            UserRole u = db.UserRole.FirstOrDefault(a => a.fid == userrole.fid);
            if (u != null)
            {
                u.fRole = userrole.fRole;
                db.SaveChanges();
            }
            return RedirectToAction("EditAccountRole");
        }
        //刪除帳號權限
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("SearchAccountRole");
            Member m = db.Member.FirstOrDefault(a => a.fMemberId == id);
            if (m != null)
            {
                db.Member.Remove(m);
                db.SaveChanges();
            }
            return RedirectToAction("SearchAccountRole");
        }
    }
}