using smallWorld.Models;
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
        public CMember newMember { get; set; }
        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        [RegularExpression(@"[a-zA-Z]+[a-zA-Z0-9]*$", ErrorMessage = "密碼僅能有英文或數字，且開頭需為英文字母！")]
        public string password { get; set; }
        [DisplayName("確認密碼")]
        [Compare("password", ErrorMessage = "兩次密碼不一致")]
        [Required(ErrorMessage = "請再次輸入密碼")]
        public string password_confirm { get; set; }
    }
}