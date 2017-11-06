using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Android.Gms.Maps;
using Android.Support.V4.App;
using Android.Gms.Maps.Model;

using Android.Content.PM;
using System.Globalization;
using Plugin.Connectivity;
using System.Net.Http;
using UberClone.Helpers;
using UberClone.Models;
using System.Threading.Tasks;

namespace UberClone.Activities
{
    [Activity(Label = "Client Location", ScreenOrientation =ScreenOrientation.Portrait, Theme = "@style/Theme.MyCustomTheme")]
    public class ActivityViewRiderLocation : FragmentActivity, ILocationListener, IOnMapReadyCallback, GoogleMap.IOnMapLoadedCallback
    {
        private GoogleMap mMap; //Null if google apk services isn't available...
        public LocationManager locationmanager;
        public string provider;
        public Location location;
        CameraUpdate drivercamera;

        Button acceptrequest, buttonback;

        int request_id;
        double requesterlongitude, requesterlatitude;
        string requesterusername, driverusername;

        List<Marker> markers = new List<Marker>();
        LatLngBounds.Builder builder;

        Handler handler = new Handler();
        Action act;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.Layout_ActivityViewRiderLocation);
                buttonback = FindViewById<Button>(Resource.Id.button_back);
                acceptrequest = FindViewById<Button>(Resource.Id.button_acceptrequest);
                request_id = Intent.GetIntExtra("request_id", 0);
                requesterusername = Intent.GetStringExtra("username");
                requesterlatitude = Intent.GetDoubleExtra("latitude", 0);
                requesterlongitude = Intent.GetDoubleExtra("longitude", 0);
                driverusername = Intent.GetStringExtra("driver_usename");


                SetUpMapIfNeeded();
              

                acceptrequest.Click += Acceptrequest_Click;
                buttonback.Click += Buttonback_Click;

                locationmanager = (LocationManager)GetSystemService(Context.LocationService);
                provider = locationmanager.GetBestProvider(new Criteria(), false);
                locationmanager.RequestLocationUpdates(provider, 400, 1, this);
                location = locationmanager.GetLastKnownLocation(provider);

                act = new Action(UpdateLocation);
            }
            catch (Exception ex)
            {

                Android.Util.Log.Info("UberCloneApp.ActivityViewRiderLocation.OnCreate", ex.InnerException.Message);
            }


            
        }

        private void Buttonback_Click(object sender, EventArgs e)
        {
            //Intent i = new Intent(this, typeof(ActivityViewRequests));
            //this.StartActivity(i);
            OnBackPressed();
        }

        private async void Acceptrequest_Click(object sender, EventArgs e)
        {
            Tuple<bool,string> result = await AcceptRequestUpdateRiderRequest();
            if (!result.Item1)
            {
               
            }
            if (result.Item1)
            {
                Intent intent = new Intent(Intent.ActionView,
               Android.Net.Uri.Parse("http://maps.google.com/maps?daddr=" + requesterlatitude.ToString(CultureInfo.InvariantCulture) + "," + requesterlongitude.ToString(CultureInfo.InvariantCulture)));
                StartActivity(intent);
            }
        }

        private async Task<Tuple<bool, string>> AcceptRequestUpdateRiderRequest()
        {
            //check internet first
            if (CrossConnectivity.Current.IsConnected)
            {
                //internet available, setting up locals & save 'em to db
                string requestid = request_id.ToString();
                string reqLong = requesterlongitude.ToString(CultureInfo.InvariantCulture);
                string reqlat = requesterlatitude.ToString(CultureInfo.InvariantCulture);
                string myusername = Settings.Username;
                FormUrlEncodedContent paramss = new FormUrlEncodedContent(new[]
               {
                    new KeyValuePair<string, string>("request_id",requestid),
                    new KeyValuePair<string, string>("requester_username",requesterusername),
                    new KeyValuePair<string, string>("requester_longitude",reqLong),
                    new KeyValuePair<string, string>("requester_latitude",reqlat),
                    new KeyValuePair<string, string>("driver_usename",myusername)

                 });
                    Tuple<Request,bool,string> result = await RestHelper.APIRequest<Request>(AppUrls.api_url_requests+requestid, HttpVerbs.PUT, null,null, paramss);

                    return new Tuple<bool, string>(result.Item2, result.Item3);
            }
            else
            {
                //internet not available, user tries again later
                return new Tuple<bool, string>(false, "No Internet Connection!");
            }
        }

        public void OnLocationChanged(Location location)
        {
            location = locationmanager.GetLastKnownLocation(provider);
            Toast.MakeText(this, "Location Changed", ToastLength.Short);
        }

        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "Provider Disabled", ToastLength.Short);
        }

        public void OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "Provider Enabled", ToastLength.Short);
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Toast.MakeText(this, "Status Changed", ToastLength.Short);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.mMap = googleMap;

            locationmanager = (LocationManager)GetSystemService(Context.LocationService);
            provider = locationmanager.GetBestProvider(new Criteria(), false);
            locationmanager.RequestLocationUpdates(provider, 400, 1, this);
            UpdateLocation();
        }

        private void UpdateLocation()
        {
            try
            {
                mMap.Clear();
                markers.Clear();
                builder = new LatLngBounds.Builder();
                location = locationmanager.GetLastKnownLocation(provider);
                if (location != null)
                {
                    LatLng driverlatlng = new LatLng(location.Latitude, location.Longitude);
                    markers.Add(mMap.AddMarker(new MarkerOptions()
                        .SetPosition(driverlatlng)
                        .SetTitle("MyLocation")));
                }
                else
                {
                    Toast.MakeText(this, "Couldn't Locate Your Position", ToastLength.Short);
                }
                if (requesterlatitude != 0 & requesterlongitude != 0)
                {
                    LatLng riderlatlng = new LatLng(requesterlatitude, requesterlongitude);
                    markers.Add(mMap.AddMarker(new MarkerOptions()
                        .SetIcon(BitmapDescriptorFactory
                        .DefaultMarker(BitmapDescriptorFactory.HueBlue))
                        .SetPosition(riderlatlng)
                        .SetTitle("RiderLocation")));
                }
                else
                {
                    Toast.MakeText(this, "Couldn't Locate Client Position", ToastLength.Short);
                }

                if (markers.Count > 0)
                {
                    foreach (Marker m in markers)
                    {
                        builder.Include(m.Position);
                    }

                }

                mMap.SetOnMapLoadedCallback(this);

                handler.PostDelayed(new Java.Lang.Runnable(act), 5000);
                Android.Util.Log.Info("Lift", "Handler");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short);
            }
        }

        private void SetUpMapIfNeeded()
        {
            // Do a null check to confirm that we have not already instantiated the map.
            if (mMap == null)
            {
                SupportMapFragment frag = (SupportFragmentManager.FindFragmentById(Resource.Id.fragment_riderlocationmap) as SupportMapFragment);
                frag.GetMapAsync(this);
            }
        }

        public void OnMapLoaded()
        {
            mMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(builder.Build(), 100));
        }
    }
}