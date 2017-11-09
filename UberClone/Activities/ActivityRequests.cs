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

using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Support.V7.App;
using Android.Support.V4.App;
using UberClone.Models;
using UberClone.Helpers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Globalization;
using Java.Util;
using Android.Graphics;
using System.Threading.Tasks;
using Java.Lang;

namespace UberClone.Activities
{
    [Activity(Label = "ActivityRequests",ScreenOrientation =Android.Content.PM.ScreenOrientation.Portrait)]
    public class ActivityRequests : FragmentActivity ,Android.Locations.ILocationListener,Android.Gms.Maps.IOnMapReadyCallback, GoogleMap.IOnMarkerClickListener
    {
        public GoogleMap mMap;
        public LocationManager locationmanager;
        public string provider;
        public Location location;

        public List<Marker> markers = new List<Marker>();
        public List<Request> List_Request = new List<Request>();
        public Dictionary<Marker, Request> myMarkers = new Dictionary<Marker, Request>();
        public PolylineOptions myPolyLineOptions = new PolylineOptions();

        public LatLngBounds.Builder builder;

        public Handler myHandler = new Handler();
        public Action myAction;
        public Runnable myRunnable;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_ActivityRequests);


            SetupMap();

            myAction = new Action(UpdateMap);
            myRunnable = new Runnable(myAction);
            
        }

        private void UpdateMap()
        {
            location = locationmanager.GetLastKnownLocation(provider);
            if (location != null)
            {
                RefreshMapMarkers();
                SetMyLocation();


            }
            myHandler.PostDelayed(myRunnable, 5000);
        }

        private async void RefreshMapMarkers()
        {
            mMap.Clear();
            markers.Clear();
            builder = new LatLngBounds.Builder();

            Tuple<List<Request>, bool, string> requests = await RestHelper.APIRequest<List<Request>>(AppUrls.api_url_GetRequestsWithoutDriver, HttpVerbs.GET, null, null);
            if (requests.Item1.Count>0)
            {
                List_Request.Clear();
                markers.Clear();
                myMarkers.Clear();
                foreach (Request i in requests.Item1)
                {
                    if (string.IsNullOrEmpty(i.driver_usename))
                    {
                        LatLng driverlatlng = new LatLng(location.Latitude, location.Longitude);
                        markers.Add(mMap.AddMarker(new MarkerOptions()
                            .SetIcon(BitmapDescriptorFactory
                            .DefaultMarker(BitmapDescriptorFactory.HueRed))
                            .SetPosition(driverlatlng)
                            .SetTitle("MyLocation")));
                        
                        if (i.requester_latitude != 0 & i.requester_longitude != 0)
                        {
                            double distanceinkm = GeoDistanceHelper.DistanceBetweenPlaces(location.Longitude, location.Latitude, i.requester_longitude, i.requester_latitude);

                            LatLng riderlatlng = new LatLng(i.requester_latitude, i.requester_longitude);

                            Marker marker = mMap.AddMarker(new MarkerOptions()
                                .SetIcon(BitmapDescriptorFactory
                                .DefaultMarker(BitmapDescriptorFactory.HueOrange))
                                .SetPosition(riderlatlng)
                                .SetTitle(i.requester_username));
                            
                            Request Req = new Request
                            {
                                request_id = i.request_id,
                                requester_username = i.requester_username,
                                driver_usename = i.driver_usename,
                                requester_longitude = i.requester_longitude,
                                requester_latitude = i.requester_latitude
                            };

                            markers.Add(marker);
                            List_Request.Add(Req);

                             myMarkers.Add(marker,Req);
                        }
                    }
                }
                if (markers.Count > 0)
                {
                    foreach (Marker m in markers)
                    {
                        builder.Include(m.Position);
                    }

                }
                LatLngBounds bounds = builder.Build();
                CameraUpdate cu = CameraUpdateFactory.NewLatLngBounds(bounds, 100);
                mMap.MoveCamera(cu);
            }
            else
            {
                LatLng driverlatlng = new LatLng(location.Latitude, location.Longitude);
                markers.Add(mMap.AddMarker(new MarkerOptions()
                    .SetIcon(BitmapDescriptorFactory
                    .DefaultMarker(BitmapDescriptorFactory.HueRed))
                    .SetPosition(driverlatlng)
                    .SetTitle("MyLocation")));
                CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(driverlatlng, 18);
                mMap.MoveCamera(camera);

                Toast.MakeText(this, "No Requests Found :(", ToastLength.Short).Show();
                Android.Util.Log.Info("Lift_API", "No_Requests_Found");
            }

        }
        public bool OnMarkerClick(Marker marker)
        {
            myHandler.RemoveCallbacks(myRunnable);
            var marker_request = myMarkers.Where(x => x.Key.Title == marker.Title).Select(x => x.Value).SingleOrDefault();
            if (marker_request!=null)
            {
                LatLng location_driver = new LatLng(location.Latitude, location.Longitude);
                LatLng location_client = new LatLng(marker_request.requester_latitude, marker_request.requester_longitude);
                Android.Util.Log.Info("Lift_Directions_OnMarkerClick", Convert.ToString(location_client) + Convert.ToString(location_driver));
                GetPolyline(location_driver,location_client);
                
                return true;
            }
            else
            {
                myHandler.PostDelayed(myRunnable, 0);
                return false;
            }
        }
        public async void GetPolyline(LatLng a, LatLng b)
        {
            var polyoptions = new PolylineOptions();
            polyoptions.Add(new LatLng(33.493989, -7.728172));
            polyoptions.Add(new LatLng(33.615988, -7.499647));
            var daline = mMap.AddPolyline(polyoptions);
            daline.Width = 20;
            daline.Color = Color.ParseColor("#0099ff");

            Android.Util.Log.Info("Lift_INFO", "Google Location Enabled: "+ mMap.MyLocationEnabled.ToString());
            string a_latitude = Convert.ToString(a.Latitude);
            string a_logitude = Convert.ToString(a.Longitude);
            string b_latitude = Convert.ToString(b.Latitude);
            string b_longitude = Convert.ToString(b.Longitude);
            string url = "https://maps.googleapis.com/maps/api/directions/json"
                + "?origin="
                + a_latitude
                + ","
                + a_logitude
                + "&destination="
                + b_latitude
                + ","
                + b_longitude
                + "&key=AIzaSyAZRBPXKfwvmTTD0nFkfsweU3OhLAQhGC8";

            Tuple<Direction, bool, string> result_directions =
                await RestHelper.APIRequest<Direction>(url, HttpVerbs.GET);
            if (!result_directions.Item2)
            {
                Toast.MakeText(this, "Error_Directions_No_Response :(", ToastLength.Short).Show();
                Android.Util.Log.Info("Lift_API", "Error_Directions_No_Response");
            }
            if (result_directions.Item2)
            {
                if (result_directions.Item1.status == "NOT_FOUND")
                {
                    Toast.MakeText(this, "No_Directions_Found :(", ToastLength.Short).Show();
                    Android.Util.Log.Info("Lift_API", "No_Directions_Found");
                }
                if (result_directions.Item1.status == "OK")
                {
                    List<Route> route = result_directions.Item1.routes.ToList();
                    if (route.Count > 0)
                    {
                        string polylinestring = route[0].overview_polyline.points;
                        if (!string.IsNullOrEmpty(polylinestring))
                        {
                            List<LatLng> list_location = PolyLineHelper.DecodePolylinePoints(polylinestring);
                            if (list_location.Count > 0)
                            {
                                foreach (LatLng loc in list_location)
                                {
                                    myPolyLineOptions = myPolyLineOptions.Add(loc);
                                }

                                Polyline line = mMap.AddPolyline(myPolyLineOptions);
                                line.Width = 11;
                                line.Color = Color.ParseColor("#0099ff");



                            }
                        }
                    }
                }
            }
        }
        private async void SetMyLocation()
        {
            string myLong = location.Longitude.ToString(CultureInfo.InvariantCulture);
            string myLat = location.Latitude.ToString(CultureInfo.InvariantCulture);

            FormUrlEncodedContent paramss = new FormUrlEncodedContent(new[] {
                 new KeyValuePair<string, string>("user_id",Settings.User_ID),
                 new KeyValuePair<string, string>("username",Settings.Username),
                 new KeyValuePair<string, string>("usertype",Settings.Usertype),
                 new KeyValuePair<string, string>("user_longitude",myLong),
                 new KeyValuePair<string, string>("user_latitude",myLat)});

            Tuple<User, bool, string> result = await RestHelper.APIRequest<User>(AppUrls.api_url_users + Settings.User_ID, HttpVerbs.POST, null, paramss);
            if (!result.Item2)
            {
                
                Android.Util.Log.Info("Lift_API", "Error_Setting_Location");
            }
            if (result.Item2)
            {
                
                Android.Util.Log.Info("Lift_API", "Success_Setting_Location");
            }

        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.mMap = googleMap;
            locationmanager = (LocationManager)GetSystemService(Context.LocationService);
            provider = locationmanager.GetBestProvider(new Criteria(), false);
            locationmanager.RequestLocationUpdates(provider, 100, 1, this);
            mMap.SetOnMarkerClickListener(this);
            UpdateMap();

        }
        public void OnLocationChanged(Location location)
        {
            location = locationmanager.GetLastKnownLocation(provider);
        }

        public void OnProviderDisabled(string provider)
        {
            Android.Util.Log.Info("Lift_Location", "OnProviderDisabled");
        }

        public void OnProviderEnabled(string provider)
        {
            Android.Util.Log.Info("Lift_Location", "OnProviderEnabled");
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Android.Util.Log.Info("Lift_Location", "OnStatusChanged");
        }
        private void SetupMap()
        {
            if (mMap == null)
            {
                (SupportFragmentManager.FindFragmentById(Resource.Id.fragment_ActivityRequests) as SupportMapFragment).GetMapAsync(this);
            }
        }

        
    }
}