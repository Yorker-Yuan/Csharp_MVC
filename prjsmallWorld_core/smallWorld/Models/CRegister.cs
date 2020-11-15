using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace smallWorld.Models
{
    public class CRegister
    {
        [Key]
        public int fMemberId { get; set; }

        [Display(Name = "帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        public string fAccount { get; set; }

        [Display(Name ="密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        public string fPassword { get; set; }

        [Display(Name ="再次輸入密碼")]
        [Required(ErrorMessage ="請再次輸入密碼")]
        [Compare("fPassword",ErrorMessage ="密碼不一致")]
        [DataType(DataType.Password)]
        public string fPassword_confirm { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "請輸入姓名")]
        public string fName { get; set; }

        [Display(Name = "生日")]
        [Required(ErrorMessage = "請輸入生日")]
        [DataType(DataType.Date)]
        public DateTime fBirthday { get; set; }

        [Display(Name = "信箱")]
        [Required(ErrorMessage = "請輸入信箱")]
        [DataType(DataType.EmailAddress)]
        public string fEmail { get; set; }

        public DateTime fBuildtime { get; set; }

        public string fAuthcode { get; set; }
    }
}
