using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESPAdmin.Models
{
    public class AdminUsers
    {
        public string SAPID { get; set; }
        public string Role { get; set; }
        public int Type { get; set; }
    }
}