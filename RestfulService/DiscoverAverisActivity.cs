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
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Views.InputMethods;

namespace MyAveris.Droid
{
    [Activity(Label = "MYAveris", Theme = "@style/MyTheme.Base", Icon = "@drawable/icon", WindowSoftInputMode = SoftInput.StateHidden, ScreenOrientation = ScreenOrientation.Portrait)]
    public class DiscoverAverisActivity : AppCompatActivity
    {
        private int LOGIN = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Window.RequestFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.DiscoverAveris);

            // Initialize toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.AppBar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.Empty);
            SupportActionBar.SetIcon(Resource.Drawable.Icon);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            toolbar.FindViewById<TextView>(Resource.Id.lblLogin).Visibility = ViewStates.Visible;
            toolbar.FindViewById<TextView>(Resource.Id.lblLogin).Click += LoginButton_OnClick;
        }

        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
                var currentFocus = this.CurrentFocus;
                if (currentFocus != null)
                {
                    inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
                }
            }
            catch { }
        }

        void LoginButton_OnClick(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(LoginActivity));
            StartActivityForResult(intent, LOGIN);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == LOGIN)
            {
                if (resultCode == Result.Ok)
                {
                    var main = new Intent(this, typeof(MainActivity));
                    StartActivity(main);
                    Finish();
                }
            }
        }
    }
}