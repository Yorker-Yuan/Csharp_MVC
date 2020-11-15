using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using smallWorld.Models;

namespace smallWorld.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CRegister member)
        {
            string connection = "Data Source=LAPTOP-49GQ7CD1\\SQLEXPRESS;Initial Catalog=dbCustomer;Integrated Security=True";
            using (SqlConnection con = new SqlConnection(connection))
            {
                string sqlQuery = $@"Insert into Member(fAccount,fPassword,fName,fBirthday,fEmail,fBuildtime)
                    values ('{member.fAccount}','{member.fPassword}','{member.fName}',
'{member.fBirthday.ToString("yyyy-MM-dd")}','{member.fEmail}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    ViewData["Message"] = "會員" +"  "+member.fName + "  " +  "註冊成功...!";
                }
            }
                return View(member);
        }
    }
}
