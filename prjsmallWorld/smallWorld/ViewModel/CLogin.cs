using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace smallWorld.ViewModel
{
    public class CLogin
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage ="輸入帳號")]
        public string account { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage ="輸入密碼")]
        [DataType(DataType.Password)]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "密碼長度介於6-10字元")]
        [RegularExpression(@"[a-zA-Z]+[a-zA-Z0-9]*$", ErrorMessage = "密碼僅能有英文或數字，且開頭需為英文字母！")]
        public string password { get; set; }

    }
}