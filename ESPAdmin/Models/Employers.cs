using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESPAdmin.Models
{
    public class Employers
    {
        public string EmployerID { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string Email { get; set; }

        [Display(Name = "Relationship Manager")]
        public string RelationshipManager { get; set; }

        [Display(Name = "RM Name")]
        public string RMName { get; set; }
    }
}