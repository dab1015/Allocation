using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNRWMSPortal.Models
{
    public class UserInfoList
    {
        public List<UserInfosDetail> UserInfos { get; set; }
    }

    public class UserInfosDetail
    {
        public string Username { get; set; }
        public string userFirstName { get; set; }
        public string userLastName { get; set; }
        public string userProfile { get; set; }
        public string isProfileAdmin { get; set; }
    }
}
