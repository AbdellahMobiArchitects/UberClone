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
using Android.Support.V4.App;
using System.Collections.Generic;

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
        private async void Button_getstarted_Click(object sender, EventArgs e)
        {

            if (switch_usertype.Checked)
            {
                RiderOrDriver = switch_usertype.TextOn;
                if (string.IsNullOrEmpty(Settings.User_ID))
                {
                    var saveresult = await SaveUser();
                    if (saveresult.Item2)
                    {
                        RedirectUser(typeof(ActivityViewRequests));
                    }
                    else
                    {
                        Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                        Android.App.AlertDialog alert = dialog.Create();
                        alert.SetTitle("Information!");
                        alert.SetMessage("Couldn't Add You To Database");
                        alert.SetIcon(Resource.Drawable.alert);
                        alert.SetButton("OK", (c, ev) =>
                        {

                        });
                        alert.Show();
                    }
                }
                if (!string.IsNullOrEmpty(Settings.User_ID))
                {
                    RedirectUser(typeof(ActivityViewRequests));
                }
            }
            if (!switch_usertype.Checked)
            {
                RiderOrDriver = switch_usertype.TextOff;
                if (string.IsNullOrEmpty(Settings.User_ID) )
                {
                    var saveresult = await SaveUser();
                    if (saveresult.Item2)
                    {
                        Settings.User_ID = saveresult.Item1.user_id.ToString();
                        Settings.Username = saveresult.Item1.username;
                        Settings.Usertype = saveresult.Item1.usertype;
                        Settings.User_Longitude = saveresult.Item1.user_longitude.ToString();
                        Settings.User_Latitude = saveresult.Item1.user_latitude.ToString();
                        RedirectUser(typeof(ActivityYourLocation));
                    }
                }
                if (!string.IsNullOrEmpty(Settings.User_ID))
                {
                    RedirectUser(typeof(ActivityYourLocation));
                }
                
            }

        }
        private void RedirectUser(Type activity)
        {
            Intent i = new Intent(this, activity);
            i.PutExtra("user_type", RiderOrDriver);
            this.StartActivity(i);
        }

        #region Onkeybackpressed
        public override void OnBackPressed()
        {
            //bad idea deleting user from here; he can just kill process without doing this work
            //TODO: implement a good userdeletion maybe after the request has been fulfilled
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Info");
            alert.SetMessage("Exit App?");
            alert.SetIcon(Resource.Drawable.alert);
            alert.SetButton("OK", async (c, ev) =>
             {
                 //base.OnBackPressed();
                 if (await DeleteUser())
                 {
                     if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                     { FinishAndRemoveTask(); }
                     else { Finish(); }
                 }
             });
            alert.SetButton2("CANCEL", (c, ev) => {/* here cancel action */ });
            alert.Show();
        }
        #endregion

        #region SaveUserToDB
        private async Task<Tuple<User,bool, string>> SaveUser()
        {
            //check internet first
            if (CrossConnectivity.Current.IsConnected)
            {
                //internet available, save2db & set up localc params
                var postparams = new FormUrlEncodedContent(new[]
               {
                     new KeyValuePair<string, string>("usertype", RiderOrDriver)
                 });
                var result = await RestHelper.APIRequest<User>(AppUrls.api_url_users, HttpVerbs.POST, null, postparams,null);
                if (result.Item1 != null & result.Item2)
                {
                    return new Tuple<User, bool, string>(result.Item1, result.Item2, result.Item3);
                }
                else
                {
                    return new Tuple<User, bool, string>(result.Item1, result.Item2, result.Item3);
                }
            }
            else
            {
                //internet not available, user tries again later
                return new Tuple<User, bool, string>(default(User),false, "No Internet Connection!");
            }
        }
        #endregion

        #region DeleteUserFromDB

        private async Task<bool> DeleteUser()
        {
            if (!string.IsNullOrEmpty(Settings.User_ID))
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    //attempting user deletion from db
                    string url = AppUrls.api_url_users + Settings.User_ID;
                    var httpClient = new HttpClient();
                    var response = await httpClient.DeleteAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        //successful attempt, cleaning local variables as well
                        //Settings.ClearUserLocalVars();
                        Toast.MakeText(this, "Cya Next Time!", ToastLength.Short).Show();
                        return true;
                    }
                    else
                    {
                        //failed attempt, app stays open for now
                        Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                        Android.App.AlertDialog alert = dialog.Create();
                        alert.SetTitle("Information!");
                        alert.SetMessage("Couldn't Clean User From Database!");
                        alert.SetIcon(Resource.Drawable.alert);
                        alert.SetButton("OK", (c, ev) =>
                        {

                        });
                        alert.Show();
                        return false;
                    }
                }
                else
                {
                    Toast.MakeText(this, "No Connection, Couldn't Clean User From Database!", ToastLength.Long).Show();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}