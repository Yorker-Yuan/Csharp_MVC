using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace smallWorld.Services
{
    public class MailService
    {
        private string gmail_account = "";
        private string gmail_password = "";
        private string gmail_mail = "";

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