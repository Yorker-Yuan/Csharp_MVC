using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace smallWorld.ViewModel
{
    public class CData
    {
        public IEnumerable<Member> memberData { get; set; }
        public IEnumerable<UserRole> userData { get; set; }
    }
}