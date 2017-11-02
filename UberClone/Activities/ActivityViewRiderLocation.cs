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

        double riderlongitude, riderlatitude;
        string riderusername;

        List<Marker> markers = new List<Marker>();
        LatLngBounds.Builder builder = new LatLngBounds.Builder();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SetContentView(Resource.Layout.Layout_ActivityViewRiderLocation);
                buttonback = FindViewById<Button>(Resource.Id.button_back);
                acceptrequest = FindViewById<Button>(Resource.Id.button_acceptrequest);

                riderusername = Intent.GetStringExtra("username");
                riderlatitude = Intent.GetDoubleExtra("latitude", 0);
                riderlongitude = Intent.GetDoubleExtra("longitude", 0);

                SetUpMapIfNeeded();
                locationmanager = (LocationManager)GetSystemService(Context.LocationService);
                provider = locationmanager.GetBestProvider(new Criteria(), false);
                locationmanager.RequestLocationUpdates(provider, 400, 1, this);
                location = locationmanager.GetLastKnownLocation(provider);

                acceptrequest.Click += Acceptrequest_Click;
                buttonback.Click += Buttonback_Click;
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
            var result = await AcceptRequestUpdateRiderRequest();
            if (!result.Item1)
            {
               
            }
            if (result.Item1)
            {
                Intent intent = new Intent(Intent.ActionView,
               Android.Net.Uri.Parse("http://maps.google.com/maps?daddr=" + riderlatitude.ToString(CultureInfo.InvariantCulture) + "," + riderlongitude.ToString(CultureInfo.InvariantCulture)));
                StartActivity(intent);
            }
        }

        private async Task<Tuple<bool, string>> AcceptRequestUpdateRiderRequest()
        {
            //check internet first
            if (CrossConnectivity.Current.IsConnected)
            {
                //internet available, setting up locals & save 'em to db

                var requestparameters = new FormUrlEncodedContent(new[]
               {
                    new KeyValuePair<string, string>("requester_username",riderusername),
                    new KeyValuePair<string, string>("driver_usename",Settings.Username)

                 });
                var result = await RestHelper.APIRequest<Request>(AppUrls.api_url_requests, HttpVerbs.POST, null, requestparameters);
                if (result.Item1 != null & result.Item2)
                {
                    return new Tuple<bool, string>(result.Item2, result.Item3);
                }
                else
                {
                    return new Tuple<bool, string>(result.Item2, result.Item3);
                }
            }
            else
            {
                //internet not available, user tries again later
                return new Tuple<bool, string>(false, "No Internet Connection!");
            }
        }

        public void OnLocationChanged(Location location)
        {
            UpdateLocation();
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
                    Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert = dialog.Create();
                    alert.SetTitle("Information!");
                    alert.SetMessage("Couldn't Locate Your Position");
                    alert.SetIcon(Resource.Drawable.alert);
                    alert.SetButton("OK", (c, ev) =>
                    {

                    });
                    alert.Show();
                }
                if (riderlatitude!=0 & riderlongitude!=0)
                {
                    LatLng riderlatlng = new LatLng(riderlatitude, riderlongitude);
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
                    foreach (var m in markers)
                    {
                        builder.Include(m.Position);
                    }

                }

                mMap.SetOnMapLoadedCallback(this);
                
            }
            catch (Exception ex)
            {
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("Information!");
                alert.SetMessage(ex.InnerException.Message);
                alert.SetIcon(Resource.Drawable.alert);
                alert.SetButton("OK", (c, ev) =>
                {

                });
                alert.Show();
            }
        }

        private void SetUpMapIfNeeded()
        {
            // Do a null check to confirm that we have not already instantiated the map.
            if (mMap == null)
            {
                var frag = (SupportFragmentManager.FindFragmentById(Resource.Id.fragment_riderlocationmap) as SupportMapFragment);
                frag.GetMapAsync(this);
            }
        }

        public void OnMapLoaded()
        {
            mMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(builder.Build(), 100));
        }
    }
}