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
using Android.Locations;
using Android.Gms.Maps;
using Android.Support.V4.App;
using Android.Gms.Maps.Model;

namespace UberClone.Activities
{
    [Activity(Label = "ActivityViewRiderLocation", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/Theme.MyCustomTheme")]
    public class ActivityViewRiderLocation : FragmentActivity, ILocationListener, IOnMapReadyCallback
    {
        private GoogleMap mMap; //Null if google apk services isn't available...
        public LocationManager locationmanager;
        public string provider;
        public Location location;

        Button acceptrequest, buttonback;

        double longitude, latitude;
        string username;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_ActivityViewRiderLocation);
            buttonback = FindViewById<Button>(Resource.Id.button_back);
            acceptrequest = FindViewById<Button>(Resource.Id.button_acceptrequest);

            username = Intent.GetStringExtra("username");
            latitude = Intent.GetDoubleExtra("latitude", 0);
            longitude = Intent.GetDoubleExtra("longitude", 0);

            SetUpMapIfNeeded();
            locationmanager = (LocationManager)GetSystemService(Context.LocationService);
            provider = locationmanager.GetBestProvider(new Criteria(), false);
            locationmanager.RequestLocationUpdates(provider, 400, 1, this);
            location = locationmanager.GetLastKnownLocation(provider);


            acceptrequest.Click += Acceptrequest_Click;
            buttonback.Click += Buttonback_Click;
        }

        private void Buttonback_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ActivityViewRequests));
            this.StartActivity(i);
        }

        private void Acceptrequest_Click(object sender, EventArgs e)
        {
                 Intent intent = new Intent(Intent.ActionView,

                Android.Net.Uri.Parse("http://maps.google.com/maps?daddr=" + latitude + "," +  longitude));
                StartActivity(intent);
        }

        public void OnLocationChanged(Location location)
        {
            if (mMap != null)
            {
                //mMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude, location.Longitude), 10));
                //mMap.AddMarker(new MarkerOptions().SetPosition(new LatLng(location.Latitude, location.Longitude)).SetTitle("My Location"));
                
                //LatLng riderlatlng = new LatLng(latitude, longitude);
                //MarkerOptions rideroptions = new MarkerOptions().SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)).SetPosition(riderlatlng).SetTitle("RiderLocation");
                //mMap.AddMarker(rideroptions);

                locationmanager.RequestLocationUpdates(provider, 400, 1, this);
                //mMap.Clear();
                //mMap.UiSettings.ZoomControlsEnabled = true;
                //LatLng driverlatlng = new LatLng(location.Latitude, location.Longitude);
                //MarkerOptions driveroptions = new MarkerOptions().SetPosition(driverlatlng).SetTitle("DriverLocation");
                //mMap.AddMarker(driveroptions);
                //CameraUpdate drivercamera = CameraUpdateFactory.NewLatLngZoom(driverlatlng, 20);
                //mMap.MoveCamera(drivercamera);
            }
        }

        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "ProviderDisabled", ToastLength.Long).Show();
        }

        public void OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "ProviderEnabled", ToastLength.Long).Show();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Toast.MakeText(this, "StatusChanged", ToastLength.Long).Show();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.mMap = googleMap;

            if ( mMap != null)
            {

                //mMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude, location.Longitude), 10));
                //mMap.AddMarker(new MarkerOptions().SetPosition(new LatLng(location.Latitude, location.Longitude)).SetTitle("My Location"));
                mMap.Clear();
                LatLng riderlatlng = new LatLng(latitude, longitude);
                MarkerOptions rideroptions = new MarkerOptions().SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)).SetPosition(riderlatlng).SetTitle("RiderLocation");
                mMap.AddMarker(rideroptions);

                locationmanager.RequestLocationUpdates(provider, 400, 1, this);
                mMap.UiSettings.ZoomControlsEnabled = true;
                LatLng driverlatlng = new LatLng(location.Latitude, location.Longitude);
                MarkerOptions driveroptions = new MarkerOptions().SetPosition(driverlatlng).SetTitle("DriverLocation");
                mMap.AddMarker(driveroptions);
                CameraUpdate drivercamera = CameraUpdateFactory.NewLatLngZoom(driverlatlng, 10);
                mMap.MoveCamera(drivercamera);
            }
        }
        private void SetUpMapIfNeeded()
        {
            // Do a null check to confirm that we have not already instantiated the map.
            if (mMap == null)
            {
                (SupportFragmentManager.FindFragmentById(Resource.Id.fragment_riderlocationmap) as SupportMapFragment).GetMapAsync(this);

            }
        }
    }
}