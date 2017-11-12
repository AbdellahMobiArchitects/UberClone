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
using Android.Support.V7.Widget;
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
    [Activity(Label = "ActivityRequests", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ActivityRequests : FragmentActivity, Android.Locations.ILocationListener, Android.Gms.Maps.IOnMapReadyCallback, GoogleMap.IInfoWindowAdapter
    {
        GoogleMap mMap;
        LocationManager locationmanager;
        string provider;
        Location location;

        List<Marker> markers = new List<Marker>();
        List<Request> List_Request = new List<Request>();
        Dictionary<Marker, Request> myMarkers = new Dictionary<Marker, Request>();
        PolylineOptions myPolyLineOptions;

        LatLngBounds.Builder builder;

        Handler myHandler = new Handler();
        Action myAction;
        Runnable myRunnable;


        //dialog controls
        TextView textview_estimatedtime;
        TextView textview_distance;
        Button button_acceptrequest;
        Button button_declinerequest;

        //Direction info
        Tuple<Direction, bool, string> result_directions;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_ActivityRequests);


            SetupMap();

            myAction = new Action(UpdateMap);
            myRunnable = new Runnable(myAction);


        }



        private void SetupMap()
        {
            if (mMap == null)
            {
                (SupportFragmentManager.FindFragmentById(Resource.Id.fragment_ActivityRequests) as SupportMapFragment).GetMapAsync(this);
            }
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
            Toast.MakeText(this, "Pinpointing...", ToastLength.Long);

        }

        private async void RefreshMapMarkers()
        {
            mMap.Clear();
            markers.Clear();
            builder = new LatLngBounds.Builder();

            Tuple<List<Request>, bool, string> requests = await RestHelper.APIRequest<List<Request>>(AppUrls.api_url_GetRequestsWithoutDriver, HttpVerbs.GET, null, null);
            if (requests.Item1.Count > 0)
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

                            myMarkers.Add(marker, Req);
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
            mMap.MarkerClick += mMap_MarkerClick;
            UpdateMap();

        }

        private void mMap_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            // Stop 5Sec Refresh
            myHandler.RemoveCallbacks(myRunnable);

            var marker_request = myMarkers.Where(x => x.Key.Title == e.Marker.Title).Select(x => x.Value).SingleOrDefault();
            if (marker_request != null)
            {
                LatLng location_driver = new LatLng(location.Latitude, location.Longitude);
                LatLng location_client = new LatLng(marker_request.requester_latitude, marker_request.requester_longitude);
                DrawPolyline(location_driver, location_client,mMap);
               
                e.Handled = true;
            }
            else
            {
                myHandler.PostDelayed(myRunnable, 0);
                e.Handled = false;
            }
        }
        public async void DrawPolyline(LatLng driver, LatLng rider, GoogleMap googleMap)
        {


            using (HttpClient client = new HttpClient())
            {
                myPolyLineOptions = new PolylineOptions();

                string a_latitude = Convert.ToString(driver.Latitude);
                string a_logitude = Convert.ToString(driver.Longitude);
                string b_latitude = Convert.ToString(rider.Latitude);
                string b_longitude = Convert.ToString(rider.Longitude);
                string url = "https://maps.googleapis.com/maps/api/directions/json"
                    + "?origin=" + a_latitude + "," + a_logitude
                    + "&destination=" + b_latitude + "," + b_longitude;

                HttpResponseMessage response = await client.GetAsync(url);
                string res = await response.Content.ReadAsStringAsync();
                Direction result_directions = JsonConvert.DeserializeObject<Direction>(res);

                if (result_directions.status == "OK")
                {
                    List<Route> route = result_directions.routes.ToList();

                    string polylinestring = route[0].overview_polyline.points;

                    List<LatLng> list_location = PolyLineHelper.DecodePolylinePoints(polylinestring);

                    foreach (LatLng loc in list_location)
                    {
                        myPolyLineOptions = myPolyLineOptions.Add(loc);
                    }
                    googleMap.AddPolyline(myPolyLineOptions);
                }
                else
                {
                    Toast.MakeText(this, result_directions.status, ToastLength.Long);
                    Android.Util.Log.Info("Lift_Location", result_directions.status);
                }

            }
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


        public View GetInfoContents(Marker marker)
        {
            return null;
        }

        public View GetInfoWindow(Marker marker)
        {
            return null;
        }


        public void ShowDialogRequestDecision()
        {
            try
            {

                //Inflate dialog layout
                View view = LayoutInflater.Inflate(Resource.Layout.Layout_Activity_Dialog_ActivityRequests, null);
                Android.App.AlertDialog builder = new Android.App.AlertDialog.Builder(this).Create();

                builder.SetView(view);
                builder.SetCanceledOnTouchOutside(false);
                //dialog parameters
                Window myWindow = builder.Window;
                WindowManagerLayoutParams wlp = myWindow.Attributes;
                myWindow.ClearFlags(WindowManagerFlags.DimBehind);
                wlp.Gravity = GravityFlags.Bottom;
                myWindow.Attributes = wlp;

                //Dialog controls
                textview_estimatedtime = view.FindViewById<Android.Widget.TextView>(Resource.Id.textview_estimatedtime);
                textview_distance = view.FindViewById<Android.Widget.TextView>(Resource.Id.textview_distance);
                button_acceptrequest = view.FindViewById<Android.Widget.Button>(Resource.Id.button_acceptrequest);
                button_declinerequest = view.FindViewById<Android.Widget.Button>(Resource.Id.button_declinerequest);

                //fill dialog

                textview_distance.Text = result_directions.Item1.routes[0].legs[0].distance.text;
                textview_estimatedtime.Text = result_directions.Item1.routes[0].legs[0].duration.text;


                //show dialog
                builder.Show();
                button_acceptrequest.Touch += delegate
                {

                };
                button_declinerequest.Touch += delegate
                {

                    builder.Dismiss();
                    myHandler.PostDelayed(myRunnable, 0);
                };


            }
            catch (System.Exception ex)
            {

                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Long).Show();
            }
        }

    }
}