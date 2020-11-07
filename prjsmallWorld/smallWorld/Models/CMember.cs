using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace smallWorld.Models
{
    public class CMember
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "帳號長度介於6-30字元")]
        //[Remote("accountCheck", "Member", ErrorMessage = "此帳號已註冊過")]
        public string account { get; set; }

        public string password { get; set; }
        [DisplayName("姓名")]
        [Required(ErrorMessage = "請輸入姓名")]
        [StringLength(20, ErrorMessage = "姓名長度最多20字元")]
        public string name { get; set; }
        [DisplayName("生日")]
        [Required(ErrorMessage = "請輸入生日")]
        [DataType(DataType.Date, ErrorMessage = "這不是日期格式")]
        public DateTime birthday { get; set; }
        [DisplayName("信箱")]
        [Required(ErrorMessage = "請輸入信箱")]
        [StringLength(200, ErrorMessage = "信箱長度最多200字元")]
        [EmailAddress(ErrorMessage = "這不是email格式")]
        public string email { get; set; }

        public DateTime buildTime { get; set; }
        public string authCode { get; set; }    //信箱驗證碼
        public int Role { get; set; }

    }
}