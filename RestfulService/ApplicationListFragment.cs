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
using System.ComponentModel;
using MyAverisClient;
using MyAverisEntity;
using Newtonsoft.Json;

namespace MyAveris.Droid
{
    [Activity(Label = "ApplicationListFragment")]
    public class ApplicationListFragment : Fragment
    {
        ProgressDialog _processProgress;
        String strResult;
        ListView listView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _processProgress = new ProgressDialog(this.Activity);
            _processProgress.Indeterminate = true;
            _processProgress.SetProgressStyle(ProgressDialogStyle.Spinner);
            _processProgress.SetMessage("Loading...");
            _processProgress.SetCancelable(false);
        }

        public override void OnResume()
        {
            base.OnResume();

            listView.Adapter = new CustomApplicationList(this.Activity, CacheManager.JobApplicationInfos);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ApplicationListFragment, container, false);
            listView = view.FindViewById<ListView>(Resource.Id.listJob); // get reference to the ListView in the layout
            // populate the listview with data
            listView.Adapter = new CustomApplicationList(this.Activity, CacheManager.JobApplicationInfos);
            listView.ItemClick += OnListItemClick;  // to be defined
            return view;
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _processProgress.Show();
            CacheManager.JobID = CacheManager.JobApplicationInfos[e.Position].ID;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("UserID", CacheManager.UserID.ToString()));
            headers.Add(new KeyValuePair<string, string>("TokenID", CacheManager.TokenID.ToString()));

            RestClient client = new RestClient(CacheManager.URL, HttpVerb.POST, ContentTypeString.JSON, CacheManager.JobID.ToString());
            strResult = client.ProcessRequest("GetJobApplication", headers);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                _processProgress.Dismiss();
                if (!string.IsNullOrEmpty(strResult))
                {
                    if (strResult.ToUpper().Contains("IT IS FORBIDDEN"))
                    {
                        Database.ClearData();
                        CacheManager.JobInfo = null;
                        Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this.Activity);
                        alert.SetTitle("Error");
                        alert.SetMessage("Your session has expired. Please login again.");
                        alert.SetPositiveButton("OK", (senderAlert, args) =>
                        {
                            var intent = new Intent(this.Activity, typeof(MainActivity));
                            StartActivity(intent);
                            this.Activity.FinishAffinity();
                        });

                        this.Activity.RunOnUiThread(() =>
                        {
                            alert.Show();
                        });
                    }
                    else if (strResult.Contains("ErrorMessage"))
                    {
                        if (CacheManager.JobInfo == null)
                        {
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this.Activity);
                            alert.SetTitle("Error");
                            alert.SetMessage(strResult);
                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                            {
                            });

                            this.Activity.RunOnUiThread(() =>
                            {
                                alert.Show();
                            });
                        }
                    }
                    else
                    {
                        JobApplicationInfo info = null;
                        try
                        {
                            info = JsonConvert.DeserializeObject<JobApplicationInfo>(strResult);
                        }
                        catch { }

                        if (info != null)
                        {
                            CacheManager.JobInfo = info;
                            var intent = new Intent(this.Activity, typeof(TempPersonalInfoActivity));
                            StartActivity(intent);
                        }
                        else
                        {
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this.Activity);
                            alert.SetTitle("Error");
                            alert.SetMessage("Failed to retrieve data");
                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                            {
                            });

                            this.Activity.RunOnUiThread(() =>
                            {
                                alert.Show();
                            });
                        }
                    }
                }
            }
            catch { }
        }
    }
}