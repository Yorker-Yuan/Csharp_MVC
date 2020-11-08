using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace smallWorld.ViewModel
{
    public class CRegister
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "帳號長度介於6-30字元")]
        public string account { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(10,MinimumLength =6,ErrorMessage ="密碼長度介於6-10字元")]
        [RegularExpression(@"[a-zA-Z]+[a-zA-Z0-9]*$", ErrorMessage = "密碼僅能有英文或數字，且開頭需為英文字母！")]
        public string password { get; set; }
        [DisplayName("確認密碼")]
        [Compare("password", ErrorMessage = "兩次密碼不一致")]
        [Required(ErrorMessage = "請再次輸入密碼")]
        public string password_confirm { get; set; }
        [DisplayName("姓名")]
        [Required(ErrorMessage = "請輸入姓名")]
        [StringLength(50, ErrorMessage = "姓名長度最多50字元")]
        public string name { get; set; }
        [DisplayName("生日")]
        [Required(ErrorMessage = "請輸入生日")]
        [DataType(DataType.Date, ErrorMessage = "這不是日期格式")]
        public DateTime birthday { get; set; }
        [DisplayName("信箱")]
        [Required(ErrorMessage = "請輸入信箱")]
        [StringLength(50, ErrorMessage = "信箱長度最多50字元")]
        [EmailAddress(ErrorMessage = "這不是email格式")]
        public string email { get; set; }

        public DateTime buildTime { get; set; }
        public string authCode { get; set; }    //信箱驗證碼
        public int Role { get; set; }
    }
}