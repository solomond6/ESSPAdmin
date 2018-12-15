using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESPAdmin.Models
{
    public class EmployerContacts
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int ID { get; set; }
        public string EmployerID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CustodianID { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Role { get; set; }
        public int Type { get; set; }
    }
}