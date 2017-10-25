using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Plugin.Connectivity;
using Android.Content.PM;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using UberClone.Models;
using UberClone.Helpers;
using System.Net.Http.Headers;

namespace UberClone.Activities
{
    [Activity(Label = "@string/app_name",
        MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait,
        LaunchMode = LaunchMode.SingleTop,
        Theme = "@style/Theme.MyCustomTheme",
        Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        Switch switch_usertype;
        Button button_getstarted;
        string RiderOrDriver;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Set your main view here
            SetContentView(Resource.Layout.Layout_MainActivity);
            switch_usertype = FindViewById<Switch>(Resource.Id.switch_usertype);
            button_getstarted = FindViewById<Button>(Resource.Id.button_getstarted);

            button_getstarted.Click += Button_getstarted_Click;
        }

       

        protected override void OnDestroy()
        {
            base.OnDestroy();

        }
        protected override void OnResume()
        {
            base.OnResume();
           
        }

        

        private void Button_getstarted_Click(object sender, EventArgs e)
        {

            if (switch_usertype.Checked)
            {
                RiderOrDriver = switch_usertype.TextOn;
                // SaveUserToDb(RiderOrDriver, typeof(ActivityViewRequests));
                RedirectUser(typeof(ActivityViewRequests));
                

            }
            if (!switch_usertype.Checked)
            {
                RiderOrDriver = switch_usertype.TextOff;
                //  SaveUserToDb(RiderOrDriver, typeof(ActivityYourLocation));
                RedirectUser(typeof(ActivityYourLocation));
               

            }

        }

        private void RedirectUser(Type activity)
        {
            Intent i = new Intent(this, activity);
            i.PutExtra("user_type", RiderOrDriver);
            this.StartActivity(i);
            Android.Util.Log.Info("UberCloneApp", "User is "+RiderOrDriver+" redirect to "+activity.ToString());
        }

        #region SaveUserToDB
        //private async void SaveUserToDb(string usertype, Type activity)
        //{
        //    try
        //    {
        //        if (CrossConnectivity.Current.IsConnected)
        //        {
        //            string url = AppUrls.api_url_users;
        //            User user1 = new User
        //            {
        //                usertype = usertype,
        //            };
        //            var myContent = JsonConvert.SerializeObject(user1);
        //            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //            var byteContent = new ByteArrayContent(buffer);
        //            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //            var httpClient = new HttpClient();


        //            var response = await httpClient.PostAsync(url, byteContent);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                Toast.MakeText(this, response.StatusCode.ToString() + response.ReasonPhrase.ToString(), ToastLength.Long).Show();
        //                Settings.User_id = user1.user_id.ToString();
        //                Settings.Username = user1.username;
        //                Settings.Usertype = user1.usertype;
        //                Settings.User_Lat = user1.user_latitude.ToString();
        //                Settings.User_long = user1.user_longitude.ToString();
        //                RedirectUser(activity);
        //            }
        //            else
        //            {
        //                Toast.MakeText(this, response.StatusCode.ToString() + response.ReasonPhrase.ToString(), ToastLength.Long).Show();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        Toast.MakeText(this, e.Message, ToastLength.Long).Show();

        //    }
        //}
        #endregion

        #region Onkeybackpressed
        public override void OnBackPressed()
        {
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Info");
            alert.SetMessage("Exit App?");
            alert.SetIcon(Resource.Drawable.alert);
            alert.SetButton("OK", (c, ev) =>
            {
                base.OnBackPressed();
                Android.Util.Log.Info("UberCloneApp", "Alert Ok");

                //Deleteuserfromdb(Settings.User_id);
                //Settings.ClearAll();
            });
            alert.SetButton2("CANCEL", (c, ev) => {/* here cancel action */ Android.Util.Log.Info("UberCloneApp", "Alert Cancel"); });
            alert.Show();
        }
        #endregion

        #region deleteuserfromdb
        //private async void Deleteuserfromdb(string user_id)
        //{
        //    try
        //    {
        //        if (CrossConnectivity.Current.IsConnected)
        //        {
        //            string url = AppUrls.api_url_users + user_id;
        //            var httpClient = new HttpClient();
        //            var response = await httpClient.DeleteAsync(url);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                Toast.MakeText(this, response.StatusCode.ToString() + response.ReasonPhrase.ToString(), ToastLength.Long).Show();
        //            }
        //            else
        //            {
        //                Toast.MakeText(this, response.StatusCode.ToString() + response.ReasonPhrase.ToString(), ToastLength.Long).Show();
        //            }
        //        }
        //        else
        //        {
        //            Toast.MakeText(this, "No Connection; Couldn't Clear User From DB", ToastLength.Long).Show();
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        for (int i = 0; i < 2; i++)
        //        {
        //            Toast.MakeText(this, e.Message, ToastLength.Long).Show();
        //        }

        //    }
        //}
        #endregion
    }
}

