using smallWorld.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace smallWorld.Services
{
    public class MemberService
    {
        dbCustomerEntities db = new dbCustomerEntities();
        //註冊新會員
        public void Register(CRegister member)
        {
            Member c = new Member();
            c.fBirthday = member.birthday;
            c.fEmail = member.email;
            c.fName = member.name;
            c.fBuildtime = DateTime.Now;
            c.fRole = 1;
            c.fAuthCode = Guid.NewGuid().ToString();
            //c.fPassword = HashPassword(member.fPassword);
            c.fPassword = member.password;
            db.Member.Add(c);
            db.SaveChanges();
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
        //藉由信箱取得單筆資料(全部資料)
        private CRegister getAccount(string account)
        {
            CRegister c = new CRegister();
            Member t = db.Member.Where(a => a.fAccount == account).FirstOrDefault();
            try
            {
                c.account = t.fAccount;
                c.password = t.fPassword;
                c.name = t.fName;
                c.birthday = t.fBirthday;
                c.email = t.fEmail;
                c.buildTime = DateTime.Parse(t.fBuildtime.ToString());
                c.authCode = t.fAuthCode;
                c.Role = Convert.ToInt32(t.fRole);
            }
            catch (Exception e)
            {
                //查無資料
               throw new Exception(e.Message);
            }
            return c;
        }
        //信箱驗證碼驗證
        public string emailValidation(string account, string authCode)
        {
            CRegister c = getAccount(account);
            //宣告驗證後訊息字串
            string validationStr = string.Empty;
            if (c != null)
            {
                Member t = db.Member.Where(a => a.fAccount == account && a.fAuthCode == authCode).FirstOrDefault();
                t.fAuthCode = "";
                validationStr = "信箱驗證成功，現在可以登入囉~";
            }
            return validationStr;
        }
    }
}