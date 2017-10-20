using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UberClone.Web.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int User_id { get; set; }
        public string Username { get; set; }
        public string Usertype { get; set; }
        public double User_long { get; set; }
        public double User_Lat { get; set; }

    }
}