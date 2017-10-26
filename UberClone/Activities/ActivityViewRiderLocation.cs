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

namespace UberClone.Activities
{
    [Activity(Label = "ActivityViewRiderLocation", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/Theme.MyCustomTheme")]
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
            Intent i = new Intent(this, typeof(ActivityViewRequests));
            this.StartActivity(i);
        }

        private void Acceptrequest_Click(object sender, EventArgs e)
        {
                 Intent intent = new Intent(Intent.ActionView,

                Android.Net.Uri.Parse("http://maps.google.com/maps?daddr=" + riderlatitude + "," +  riderlongitude));
                StartActivity(intent);
        }

        public void OnLocationChanged(Location location)
        {
            UpdateLocation();
            Android.Util.Log.Info("UberCloneApp.ActivityViewRiderLocation.OnLocationChanged", "Location Changed");
        }

        public void OnProviderDisabled(string provider)
        {
            Android.Util.Log.Info("UberCloneApp.ActivityViewRiderLocation.OnProviderDisabled", "Provider Disabled");
        }

        public void OnProviderEnabled(string provider)
        {
            Android.Util.Log.Info("UberCloneApp.ActivityViewRiderLocation.OnProviderEnabled", "Provider Enabled");
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Android.Util.Log.Info("UberCloneApp.ActivityViewRiderLocation.OnStatusChanged", "Status Changed");
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

                //driver & rider latlng locs
                LatLng riderlatlng = new LatLng(riderlatitude, riderlongitude);
                LatLng driverlatlng = new LatLng(location.Latitude, location.Longitude);

                ////rider marker location
                //MarkerOptions ridermarker = new MarkerOptions()
                //    .SetIcon(BitmapDescriptorFactory
                //    .DefaultMarker(BitmapDescriptorFactory.HueBlue))
                //    .SetPosition(riderlatlng)
                //    .SetTitle("RiderLocation");
                //mMap.AddMarker(ridermarker);

                ////driver marker location
                //MarkerOptions drivermarker = new MarkerOptions().SetPosition(driverlatlng).SetTitle("MyLocation");
                //mMap.AddMarker(drivermarker);

                ////move camera
                //drivercamera = CameraUpdateFactory.NewLatLngZoom(driverlatlng, 10);
                //mMap.MoveCamera(drivercamera);

                //new way

               
                markers.Add(mMap.AddMarker(new MarkerOptions()
                    .SetIcon(BitmapDescriptorFactory
                    .DefaultMarker(BitmapDescriptorFactory.HueBlue))
                    .SetPosition(riderlatlng)
                    .SetTitle("RiderLocation")));
                markers.Add(mMap.AddMarker(new MarkerOptions()
                    .SetPosition(driverlatlng)
                    .SetTitle("MyLocation")));

                foreach (var m in markers)
                {
                    builder.Include(m.Position);
                }

                mMap.SetOnMapLoadedCallback(this);
                
            }
            catch (Exception ex)
            {
                throw;
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