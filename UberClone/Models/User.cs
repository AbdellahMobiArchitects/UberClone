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
        public int user_id { get; set; }
        public string username { get; set; }
        public string usertype { get; set; }
        public Nullable<double> user_longitude { get; set; }
        public Nullable<double> user_latitude { get; set; }

        public override string ToString()
        {
            return (
                "User ID" + user_id +
                "Username" + username +
                "Usertype" + usertype +
                "User Longitude" + user_longitude +
                "User Latitude" + user_latitude
                );
        }
    }
}