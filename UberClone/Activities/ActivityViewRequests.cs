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
using UberClone.Helpers;
using Android.Locations;
using Android.Util;
using Android.Support.V4.App;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using System.Globalization;
using Plugin.Connectivity;

namespace UberClone.Activities
{
    [Activity(Label = "ViewRequests", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class ActivityViewRequests : Activity, ILocationListener
    {
        ListView lvrequests;
        LocationManager locationmanager;
        string provider;
        Location location;
        List<Request> reqs = new List<Request>();
        List<string> list_requests = new List<string>();

        Handler handler = new Handler();
        Action act;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.Layout_ViewRequests);
                lvrequests = FindViewById<ListView>(Resource.Id.listview_requests);

                locationmanager = (LocationManager)GetSystemService(Context.LocationService);
                provider = locationmanager.GetBestProvider(new Criteria(), false);
                locationmanager.RequestLocationUpdates(provider, 400, 1, this);
                location = locationmanager.GetLastKnownLocation(provider);

                FillListView();
                lvrequests.ItemClick += Lvrequests_ItemClick;

                act = new Action(FillListView);
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.InnerException.Message, ToastLength.Short);
            }
        }

        private void Lvrequests_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var selecteditem = lvrequests.GetItemAtPosition(e.Position);
            Intent i = new Intent(this, typeof(ActivityViewRiderLocation));

                var selectedrequest = reqs.ElementAt(e.Position);
                i.PutExtra("username", selectedrequest.requester_username);
                i.PutExtra("latitude", selectedrequest.requester_latitude);
                i.PutExtra("longitude", selectedrequest.requester_longitude);
                StartActivity(i);

    }
         

     
       
        private void UpdateLocation()
        {
            location = locationmanager.GetLastKnownLocation(provider);
            FillListView();
            SetMyLocation();
           
        }
        //1er Api
        private async void FillListView()
        {
            var requests = await RestHelper.APIRequest<List<Request>>(AppUrls.api_url_GetRequestsWithoutDriver, HttpVerbs.GET, null, null);
            if (requests.Item2)
            {
                reqs.Clear();
                list_requests.Clear();
                foreach (var i in requests.Item1)
                {
                    if (string.IsNullOrEmpty(i.driver_usename))
                    {

                        var distanceinkm = GeoDistanceHelper.DistanceBetweenPlaces(location.Longitude, location.Latitude, i.requester_longitude, i.requester_latitude);
                        list_requests.Add(distanceinkm.ToString() + " Km");
                        reqs.Add(new Request
                        {
                            request_id = i.request_id,
                            requester_username = i.requester_username,
                            driver_usename = i.driver_usename,
                            requester_longitude = i.requester_longitude,
                            requester_latitude = i.requester_latitude
                        });
                    }
                }
                var ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, list_requests);
                lvrequests.Adapter = ListAdapter;
            }
            else
            {
                Toast.MakeText(this, "No Requests", ToastLength.Short);
            }

            handler.PostDelayed(new Java.Lang.Runnable(act), 5000);
        }
        //2eme Api
        private async void SetMyLocation()
        {
            string myLong = location.Longitude.ToString(CultureInfo.InvariantCulture);
            string myLat = location.Latitude.ToString(CultureInfo.InvariantCulture);

            var paramss = new FormUrlEncodedContent(new[] {
                 new KeyValuePair<string, string>("user_id",Settings.User_ID),
                 new KeyValuePair<string, string>("username",Settings.Username),
                 new KeyValuePair<string, string>("usertype",Settings.Usertype),
                 new KeyValuePair<string, string>("user_longitude",myLong),
                 new KeyValuePair<string, string>("user_latitude",myLat)});

            var result = await RestHelper.APIRequest<User>(AppUrls.api_url_users + Settings.User_ID, HttpVerbs.POST, null, paramss);
            if (!result.Item2)
            {
                Toast.MakeText(this, "Update Location Error", ToastLength.Short);
            }
            if (result.Item2)
            {
                Toast.MakeText(this, "Location Updated", ToastLength.Short);
            }

            //using (HttpClient clt = new HttpClient())
            //{
            //    var response = await clt.PostAsync(AppUrls.api_url_users + Settings.User_ID, paramss);

            //    if (!response.IsSuccessStatusCode)
            //    {
            //        Toast.MakeText(this, "Update Location Error", ToastLength.Short);
            //    }
            //    if (response.IsSuccessStatusCode)
            //    {
            //        Toast.MakeText(this, "Location Updated", ToastLength.Short);
            //    }

            //}
        }

        public void OnLocationChanged(Location location)
        {
            UpdateLocation();
            Android.Util.Log.Info("UberCloneApp", "Location Changed");

        }
        public void OnProviderDisabled(string provider)
        {
            Android.Util.Log.Info("UberCloneApp", "Provider Disabled");
        }

        public void OnProviderEnabled(string provider)
        {
            Android.Util.Log.Info("UberCloneApp", "Provider Enabled");
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Android.Util.Log.Info("UberCloneApp", "Status Changed");
        }
    }
}