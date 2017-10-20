using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Content.PM;

namespace UberClone.Activities
{
    [Activity(Label = "@string/app_name",
        MainLauncher = true,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop,
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

        private void Button_getstarted_Click(object sender, EventArgs e)
        {
            
            if (switch_usertype.Checked)
            {
                RiderOrDriver = switch_usertype.TextOn;
                RedirectUser(typeof(ActivityViewRequests));
                Toast.MakeText(this, "Welcome "+RiderOrDriver, ToastLength.Short).Show();
                
            }
            if (!switch_usertype.Checked)
            {
                RiderOrDriver = switch_usertype.TextOff;
                RedirectUser(typeof(ActivityYourLocation));
                Toast.MakeText(this, "Welcome "+ RiderOrDriver, ToastLength.Short).Show();
            }
           
        }

        private void RedirectUser(Type activity)
        {
            Intent i = new Intent(this,activity);
            i.PutExtra("user_type",RiderOrDriver);
            this.StartActivity(i);
        }
    }
}

