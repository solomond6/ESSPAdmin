using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESPAdmin.Models
{
    public class InternalUsers
    {
        public string ID { get; set; }
        public string USER_NAME { get; set; }
        public string FULLNAME { get; set; }
        public string EMAIL { get; set; }
        public string LOCKED { get; set; }
        public string STATUS { get; set; }
        public string ROLE_ID { get; set; }
        public string DATE_LAST_MODIFIED { get; set; }
    }
}