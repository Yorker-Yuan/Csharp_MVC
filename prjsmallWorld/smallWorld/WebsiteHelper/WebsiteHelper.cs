using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace smallWorld.WebsiteHelper
{
    public class WebsiteHelper
    {
        //取得角色名稱
        public static string getRole
        {
            get {
                //用戶已驗證登入
                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    //使用表單驗證來辨別使用者身分
                    FormsIdentity fi = (FormsIdentity)HttpContext.Current.User.Identity;
                    //識別使用者的使用表單驗證票證的存取權
                    FormsAuthenticationTicket fat = fi.Ticket;
                    //取得用戶名稱的角色名稱字串陣列
                    string[] aryRole = fat.UserData.Split(',');
                    //傳回票證的用戶資訊字串
                    return fat.UserData;
                }
                return "";
            }
        }
    }
}