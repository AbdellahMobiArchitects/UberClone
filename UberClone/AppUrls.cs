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

namespace UberClone
{
    internal static class AppUrls
    {
       internal static string api_url = "http://192.168.1.102:6969/api/";
        internal static string api_url_users = api_url + "users/";
        internal static string api_url_GetThisUser = api_url_users + "GetThisUser/";

        internal static string api_url_requests = api_url + "requests/"; 
        internal static string api_url_GetThisUserRequest = api_url_requests + "GetThisUserRequest/";
        internal static string api_url_GetRequestsWithoutDriver = api_url_requests + "GetRequestsWithoutDriver/";
    }
}