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
using System.IO;

namespace MyAveris.Droid
{
    [Activity(Label = "MYAveris", Icon = "@drawable/icon", MainLauncher = true, NoHistory = true, Theme = "@style/Theme.Splash", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Database.newInstance(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "myaveris.db3"));
            Database.CreateTables();

            CacheManager.Init(Application.Context);

            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}