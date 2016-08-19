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
using Android.Support.V7.App;
using Android.Views.InputMethods;
using System.ComponentModel;
using MyAverisClient;
using MyAverisEntity;

namespace MyAveris.Droid
{
    [Activity(Label = "TempPersonalInfoActivity", WindowSoftInputMode = SoftInput.StateHidden, Theme = "@style/MyTheme.Base")]
    public class TempPersonalInfoActivity : AppCompatActivity
    {
        ProgressDialog _processProgress;
        string strResult;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TempPersonalInfo);

            // Initialize toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.AppBar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.PersonalInfo);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            toolbar.FindViewById<TextView>(Resource.Id.lblAction).Visibility = ViewStates.Visible;
            toolbar.FindViewById<TextView>(Resource.Id.lblAction).Click += Action_OnClick;

            _processProgress = new ProgressDialog(this);
            _processProgress.Indeterminate = true;
            _processProgress.SetProgressStyle(ProgressDialogStyle.Spinner);
            _processProgress.SetMessage("Loading...");
            _processProgress.SetCancelable(false);

            if (CacheManager.JobInfo != null)
            {
                PopulateJobApplication();

                if (CacheManager.JobInfo.IsLocked.GetValueOrDefault())
                    LockJobApplication();
            }
        }

        void Action_OnClick(object sender, EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle("Info");
            alert.SetMessage("Next course of action?");
            alert.SetPositiveButton("Hire", (senderAlert, args) =>
            {
                Finish();
            });
            alert.SetNegativeButton("Decline", (senderAlert, args) =>
            {
                _processProgress.Show();

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.RunWorkerAsync();
            });

            RunOnUiThread(() =>
            {
                alert.Show();
            });
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("UserID", CacheManager.UserID.ToString()));
            headers.Add(new KeyValuePair<string, string>("TokenID", CacheManager.TokenID.ToString()));

            RestClient client = new RestClient(CacheManager.URL, HttpVerb.POST, ContentTypeString.JSON, CacheManager.JobID.ToString());
            strResult = client.ProcessRequest("Reject", headers);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _processProgress.Dismiss();
            if (!string.IsNullOrEmpty(strResult))
            {
                _processProgress.Dismiss();

                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Error");
                alert.SetMessage(strResult);
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                });

                RunOnUiThread(() =>
                {
                    alert.Show();
                });
                return;
            }
            else
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Info");
                alert.SetMessage("Rejected successfully");
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    var item = (from info in CacheManager.JobApplicationInfos
                                where info.ID == CacheManager.JobID
                                select info).FirstOrDefault();

                    CacheManager.JobApplicationInfos.Remove(item);

                    Finish();
                });

                RunOnUiThread(() =>
                {
                    alert.Show();
                });
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        private void PopulateJobApplication()
        {
            FindViewById<RadioButton>(Resource.Id.rbIC).Checked = true;
            FindViewById<EditText>(Resource.Id.etTitle).Text = CacheManager.JobInfo.Title;
            FindViewById<EditText>(Resource.Id.etFirstName).Text = CacheManager.JobInfo.FirstName;
            FindViewById<EditText>(Resource.Id.etLastName).Text = CacheManager.JobInfo.LastName;
            FindViewById<EditText>(Resource.Id.etPositionApplied).Text = CacheManager.JobInfo.PositionApplied;
            FindViewById<EditText>(Resource.Id.etKnownAs).Text = CacheManager.JobInfo.KnownAs;
            FindViewById<EditText>(Resource.Id.etChineseCharacter).Text = CacheManager.JobInfo.ChineseCharacter;
            if (CacheManager.JobInfo.Gender == "M")
                FindViewById<RadioButton>(Resource.Id.rbMale).Checked = true;
            if (CacheManager.JobInfo.Gender == "F")
                FindViewById<RadioButton>(Resource.Id.rbFemale).Checked = true;
            if (CacheManager.JobInfo.DateOfBirth.HasValue)
                FindViewById<TextView>(Resource.Id.etDateOfBirth).Text = CacheManager.JobInfo.DateOfBirth.Value.ToString("dd-MM-yyyy");
            FindViewById<EditText>(Resource.Id.etCountryOfBirth).Text = CacheManager.JobInfo.CountryOfBirth;
            FindViewById<EditText>(Resource.Id.etNationality).Text = CacheManager.JobInfo.Nationality;
            FindViewById<EditText>(Resource.Id.etRace).Text = CacheManager.JobInfo.Race;
            FindViewById<EditText>(Resource.Id.etMaritalStatus).Text = CacheManager.JobInfo.MaritalStatus;
            FindViewById<EditText>(Resource.Id.etReligion).Text = CacheManager.JobInfo.Religion;
            FindViewById<EditText>(Resource.Id.etIdentityNo).Text = CacheManager.JobInfo.IdentityNo;
            if (CacheManager.JobInfo.DateOfIssue.HasValue)
            {
                FindViewById<RadioButton>(Resource.Id.rbPassport).Checked = true;
                FindViewById<TextView>(Resource.Id.etDateOfIssue).Text = CacheManager.JobInfo.DateOfIssue.Value.ToString("dd-MM-yyyy");
                FindViewById<LinearLayout>(Resource.Id.layDateOfIssue).Visibility = ViewStates.Visible;
                FindViewById<LinearLayout>(Resource.Id.layDateOfExpiry).Visibility = ViewStates.Visible;
                FindViewById<LinearLayout>(Resource.Id.layCountryOfIssue).Visibility = ViewStates.Visible;
            }
            if (CacheManager.JobInfo.DateOfExpiry.HasValue)
                FindViewById<TextView>(Resource.Id.etDateOfExpiry).Text = CacheManager.JobInfo.DateOfExpiry.Value.ToString("dd-MM-yyyy");
            FindViewById<EditText>(Resource.Id.etCountryOfIssue).Text = CacheManager.JobInfo.CountryOfIssue;
            FindViewById<EditText>(Resource.Id.etEmailAddress).Text = CacheManager.JobInfo.EmailAddress;
        }

        private void LockJobApplication()
        {
            FindViewById<RadioButton>(Resource.Id.rbPassport).Enabled = false;
            FindViewById<RadioButton>(Resource.Id.rbIC).Enabled = false;
            FindViewById<RadioButton>(Resource.Id.rbMale).Enabled = false;
            FindViewById<RadioButton>(Resource.Id.rbFemale).Enabled = false;

            FindViewById<EditText>(Resource.Id.etTitle).Enabled = false;
            FindViewById<EditText>(Resource.Id.etFirstName).Enabled = false;
            FindViewById<EditText>(Resource.Id.etLastName).Enabled = false;
            FindViewById<EditText>(Resource.Id.etPositionApplied).Enabled = false;
            FindViewById<EditText>(Resource.Id.etKnownAs).Enabled = false;
            FindViewById<EditText>(Resource.Id.etChineseCharacter).Enabled = false;
            FindViewById<EditText>(Resource.Id.etCountryOfBirth).Enabled = false;
            FindViewById<EditText>(Resource.Id.etNationality).Enabled = false;
            FindViewById<EditText>(Resource.Id.etRace).Enabled = false;
            FindViewById<EditText>(Resource.Id.etMaritalStatus).Enabled = false;
            FindViewById<EditText>(Resource.Id.etReligion).Enabled = false;
            FindViewById<EditText>(Resource.Id.etIdentityNo).Enabled = false;
            FindViewById<EditText>(Resource.Id.etCountryOfIssue).Enabled = false;
            FindViewById<EditText>(Resource.Id.etEmailAddress).Enabled = false;
        }
    }
}