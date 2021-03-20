using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models
{
    public class UserManagement
    {
        public int EmployeeID { get; set; }
        public int UserID { get; set; }
        public int MB_UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailID { get; set; }
        public int LastChange { get; set; }
        public int ActiveStatus { get; set; }
    }
}