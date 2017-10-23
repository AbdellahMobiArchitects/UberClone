using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace UberClone.Models
{
    public class User
    {
        public int User_id { get; set; }
        public string Username { get; set; }
        public string Usertype { get; set; }
        public double User_long { get; set; }
        public double User_Lat { get; set; }
    }
}