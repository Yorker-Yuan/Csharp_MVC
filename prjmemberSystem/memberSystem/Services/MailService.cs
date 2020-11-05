using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace memberSystem.Services
{
    public class MailService
    {
        private string gmail_account = "";
        private string gmail_password = "";
        private string gmail_mail = "";
        //產生驗證碼
        public string getValidateCode()
        {
            //設定驗證碼字元陣列
            string[] code = { "A","B","C","D","E","F","G","H","I","J","K","L","M",
                "N","P","Q","R","S","T","U","V","W","X","Y","Z","1","2","3","4","5","6","7","8","9",
                "a","b","c","d","e","f","g","h","i","j","k","l","m","n","p","q","r","s","t","u","v","w","x","y","z"};
            string validateCode = string.Empty;     //設定初始為空
            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                validateCode += code[r.Next(code.Count())];
            }
            return validateCode;
        }
        //將使用者資料填入驗證信
        public string getRegisterMailBody(string tempString, string userName, string validateUrl)
        {
            tempString = tempString.Replace("{{userName}}", userName);
            tempString = tempString.Replace("{{validateUrl}}", validateUrl);
            return tempString;
        }
        //寄信
        public void sendRegisterMail(string mailBody, string toEmail)
        {
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            //開啟SSL
            smtpServer.EnableSsl = true;

            MailMessage mail = new MailMessage();
            //設定來源信箱
            mail.From = new MailAddress(gmail_mail);
            //設定收信者信箱
            mail.To.Add(toEmail);
            //主旨
            mail.Subject = "會員註冊確認信";
            //內容
            mail.Body = mailBody;
            //設定信箱內容為HTML格式
            mail.IsBodyHtml = true;
            smtpServer.Send(mail);
        }

    }
}