using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESPAdmin.Models
{
    public class UserRoles
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}