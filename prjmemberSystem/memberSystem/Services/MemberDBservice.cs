using memberSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace memberSystem.Services
{
    public class MemberDBservice
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["db_user"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);
        //註冊
        public void register(CMember member)
        {
            //密碼hash
            member.password = hashpassword(member.password);
            //  SQL新增
            string sql = $@"INSERT INTO tUser (fAccount,fPassword,fName,fBirthday,fEmail,fBuildtime,fAuthCode,IsAdmin) 
                VALUES ('{member.account}','{member.password}','{member.name}','{member.birthday}','{member.email}',
'{DateTime.Now}','{member.authCode}','0')";
            //確保程式不會因錯誤中斷
            try
            {
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql,conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally {
                conn.Close();
            }
        }
        //hash加密
        public string hashpassword(string password)
        {
            //宣告hash時所添加的無意義字串
            string saltkey = "afsjkerhbkbg1854145";
            //結合密碼與字串
            string saltAndPassword = string.Concat(password,saltkey);
            //定義hash物件
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            //取得密碼轉成byte資料
            byte[] passwordData = Encoding.Default.GetBytes(saltAndPassword);
            //取得hash後byte物件
            byte[] hashData = sha256.ComputeHash(passwordData);
            //將hash後byte資料轉成string
            string hashResult = Convert.ToBase64String(hashData);
            return hashResult;
        }
        //藉由帳號查單筆資料
        private CMember getDataByAccount(string account)
        {
            CMember data = new CMember();
            string sql = $@"select * from tUser where fAccount = '{account}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql,conn);
                //取得sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                data.account = dr["fAccount"].ToString();
                data.password = dr["fPassword"].ToString();
                data.name = dr["fName"].ToString();
                data.email = dr["fEmail"].ToString();
                data.birthday = DateTime.Parse(dr["fBirthday"].ToString());
                data.buildTime = DateTime.Parse(dr["fBuildtime"].ToString());
                data.authCode = dr["fAuthCode"].ToString();
                data.isAdmin = Convert.ToBoolean(dr["IsAdmin"]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally {
                conn.Close();
            }
            return data;
        }
        //帳號重複註冊確認
        public bool accountCheck(string account)
        {
            CMember data = getDataByAccount(account);
            //是否查到會員
            bool result = (data== null);
            return result;
        }
        //信箱驗證碼驗證
        public string emailValidate(string account, string authcode)
        {
            CMember vmember = getDataByAccount(account);
            //驗證後訊息
            string vstr = string.Empty;
            if (vmember != null)
            {
                //判斷傳入驗證碼是否與資料庫中相同
                if (vmember.authCode == authcode)
                {
                    //將資料庫驗證碼設為空
                    //sql更新
                    string sql = $@"update tUser set fAuthCode='{string.Empty}' where fAccount = '{account}'";
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message.ToString());
                    }
                    finally
                    {
                        conn.Close();
                    }
                    vstr = "帳號信箱驗證成功，現在可以登入了!";
                }
                else {
                    vstr = "驗證碼錯誤，請重新確認後再註冊";
                }
            }
            else {
                vstr = "傳送資料錯誤，請重新確認或再註冊";
            }
            return vstr;
        }

    }
}