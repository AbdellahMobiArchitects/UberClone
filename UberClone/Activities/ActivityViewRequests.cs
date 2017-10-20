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
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Specialized;
using Newtonsoft.Json;
using UberClone.Models;

namespace UberClone.Activities
{
    [Activity(Label = "Activity_ViewRequests", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class ActivityViewRequests : Activity
    {
        ListView lvrequests;
        List<string> requestslist;

        Button request;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.Layout_ViewRequests);
                lvrequests = FindViewById<ListView>(Resource.Id.listview_requests);
                request = FindViewById<Button>(Resource.Id.button_request);
                request.Click += Request_Click;
                //requestslist = new List<string>();

                //var requests = await GetRequestsAsync("http://192.168.1.102:6969/api/requests");
                //foreach (var i in requests)
                //{
                //    reqids.Add(i.Requestid.ToString());
                //}
                //var reqadapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, reqids);
                //lvrequests.Adapter = reqadapter;


            }
            catch (Exception e)
            {
                Android.Util.Log.Info("UberClone", e.InnerException.Message);
                Toast.MakeText(this, "Error" + e.InnerException.Message, ToastLength.Long).Show();
            }
           
            

        }

        private void Request_Click(object sender, EventArgs e)
        {
            RedirectUserExtra(typeof(ActivityViewRiderLocation));
        }
        private void RedirectUserExtra(Type activity)
        {
            Intent i = new Intent(this, activity);
            i.PutExtra("username","nitro2010");
            i.PutExtra("latitude", 33.542660);
            i.PutExtra("longitude", -7.629246);
            this.StartActivity(i);
        }
        //  private async Task<List<Request>> GetRequestsAsync(string url)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        // send a GET request  
        //        var uri = url;
        //        var result = await client.GetStringAsync(uri);
        //        var reqs = JsonConvert.DeserializeObject<List<Request>>(result);
        //        return reqs;
        //    }
        //}

    }
}