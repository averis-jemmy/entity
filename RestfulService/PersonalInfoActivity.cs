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

namespace MyAveris.Droid
{
    [Activity(Label = "PersonalInfoActivity", WindowSoftInputMode = SoftInput.StateHidden, Theme = "@style/MyTheme.Base")]
    public class PersonalInfoActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PersonalInfo);

            // Initialize toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.AppBar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.PersonalInfo);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            toolbar.FindViewById<TextView>(Resource.Id.lblSave).Visibility = ViewStates.Visible;
            toolbar.FindViewById<TextView>(Resource.Id.lblSave).Click += Save_OnClick;

            FindViewById<TextView>(Resource.Id.etDateOfBirth).Click += DateOfBirthText_OnClick;
            FindViewById<TextView>(Resource.Id.etDateOfIssue).Click += DateOfIssueText_OnClick;
            FindViewById<TextView>(Resource.Id.etDateOfExpiry).Click += DateOfExpiryText_OnClick;
            FindViewById<RadioButton>(Resource.Id.rbIC).Click += RadioIdentity_OnClick;
            FindViewById<RadioButton>(Resource.Id.rbPassport).Click += RadioIdentity_OnClick;

            if (CacheManager.JobInfo != null)
            {
                PopulateJobApplication();

                if (CacheManager.JobInfo.IsLocked.GetValueOrDefault())
                    LockJobApplication();
            }
        }

        void DateOfBirthText_OnClick(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate(DateTime time)
            {
                FindViewById<TextView>(Resource.Id.etDateOfBirth).Text = time.ToString("dd-MM-yyyy");
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        void DateOfIssueText_OnClick(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate(DateTime time)
            {
                FindViewById<TextView>(Resource.Id.etDateOfIssue).Text = time.ToString("dd-MM-yyyy");
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        void DateOfExpiryText_OnClick(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate(DateTime time)
            {
                FindViewById<TextView>(Resource.Id.etDateOfExpiry).Text = time.ToString("dd-MM-yyyy");
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void RadioIdentity_OnClick(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Text == "IC")
            {
                FindViewById<LinearLayout>(Resource.Id.layDateOfIssue).Visibility = ViewStates.Gone;
                FindViewById<LinearLayout>(Resource.Id.layDateOfExpiry).Visibility = ViewStates.Gone;
                FindViewById<LinearLayout>(Resource.Id.layCountryOfIssue).Visibility = ViewStates.Gone;

                FindViewById<TextView>(Resource.Id.etDateOfIssue).Text = string.Empty;
                FindViewById<TextView>(Resource.Id.etDateOfExpiry).Text = string.Empty;
                FindViewById<EditText>(Resource.Id.etCountryOfIssue).Text = string.Empty;
            }
            else
            {
                FindViewById<LinearLayout>(Resource.Id.layDateOfIssue).Visibility = ViewStates.Visible;
                FindViewById<LinearLayout>(Resource.Id.layDateOfExpiry).Visibility = ViewStates.Visible;
                FindViewById<LinearLayout>(Resource.Id.layCountryOfIssue).Visibility = ViewStates.Visible;
            }
        }

        void Save_OnClick(object sender, EventArgs e)
        {
            UpdateJobApplication();
            Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    UpdateJobApplication();
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            UpdateJobApplication();
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
            FindViewById<TextView>(Resource.Id.etDateOfBirth).Click -= DateOfBirthText_OnClick;
            FindViewById<EditText>(Resource.Id.etCountryOfBirth).Enabled = false;
            FindViewById<EditText>(Resource.Id.etNationality).Enabled = false;
            FindViewById<EditText>(Resource.Id.etRace).Enabled = false;
            FindViewById<EditText>(Resource.Id.etMaritalStatus).Enabled = false;
            FindViewById<EditText>(Resource.Id.etReligion).Enabled = false;
            FindViewById<EditText>(Resource.Id.etIdentityNo).Enabled = false;
            FindViewById<TextView>(Resource.Id.etDateOfIssue).Click -= DateOfIssueText_OnClick;
            FindViewById<TextView>(Resource.Id.etDateOfExpiry).Click -= DateOfExpiryText_OnClick;
            FindViewById<EditText>(Resource.Id.etCountryOfIssue).Enabled = false;
            FindViewById<EditText>(Resource.Id.etEmailAddress).Enabled = false;
        }

        private void UpdateJobApplication()
        {
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

            if (!CacheManager.JobInfo.IsLocked.GetValueOrDefault())
            {
                CacheManager.JobInfo.Title = FindViewById<EditText>(Resource.Id.etTitle).Text;
                CacheManager.JobInfo.FirstName = FindViewById<EditText>(Resource.Id.etFirstName).Text;
                CacheManager.JobInfo.LastName = FindViewById<EditText>(Resource.Id.etLastName).Text;
                CacheManager.JobInfo.PositionApplied = FindViewById<EditText>(Resource.Id.etPositionApplied).Text;
                CacheManager.JobInfo.KnownAs = FindViewById<EditText>(Resource.Id.etKnownAs).Text;
                CacheManager.JobInfo.ChineseCharacter = FindViewById<EditText>(Resource.Id.etChineseCharacter).Text;
                if (FindViewById<RadioButton>(Resource.Id.rbMale).Checked)
                    CacheManager.JobInfo.Gender = "M";
                if (FindViewById<RadioButton>(Resource.Id.rbFemale).Checked)
                    CacheManager.JobInfo.Gender = "F";
                if (!string.IsNullOrEmpty(FindViewById<TextView>(Resource.Id.etDateOfBirth).Text))
                    CacheManager.JobInfo.DateOfBirth = DateTime.ParseExact(FindViewById<TextView>(Resource.Id.etDateOfBirth).Text, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                else
                    CacheManager.JobInfo.DateOfBirth = null;
                CacheManager.JobInfo.CountryOfBirth = FindViewById<EditText>(Resource.Id.etCountryOfBirth).Text;
                CacheManager.JobInfo.Nationality = FindViewById<EditText>(Resource.Id.etNationality).Text;
                CacheManager.JobInfo.Race = FindViewById<EditText>(Resource.Id.etRace).Text;
                CacheManager.JobInfo.MaritalStatus = FindViewById<EditText>(Resource.Id.etMaritalStatus).Text;
                CacheManager.JobInfo.Religion = FindViewById<EditText>(Resource.Id.etReligion).Text;
                CacheManager.JobInfo.IdentityNo = FindViewById<EditText>(Resource.Id.etIdentityNo).Text;
                if (!string.IsNullOrEmpty(FindViewById<TextView>(Resource.Id.etDateOfIssue).Text))
                    CacheManager.JobInfo.DateOfIssue = DateTime.ParseExact(FindViewById<TextView>(Resource.Id.etDateOfIssue).Text, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                else
                    CacheManager.JobInfo.DateOfIssue = null;
                if (!string.IsNullOrEmpty(FindViewById<TextView>(Resource.Id.etDateOfExpiry).Text))
                    CacheManager.JobInfo.DateOfExpiry = DateTime.ParseExact(FindViewById<TextView>(Resource.Id.etDateOfExpiry).Text, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                else
                    CacheManager.JobInfo.DateOfExpiry = null;
                CacheManager.JobInfo.CountryOfIssue = FindViewById<EditText>(Resource.Id.etCountryOfIssue).Text;
                CacheManager.JobInfo.EmailAddress = FindViewById<EditText>(Resource.Id.etEmailAddress).Text;

                JobApplication model = new JobApplication()
                {
                    ID = CacheManager.JobApplicationID,
                    Title = CacheManager.JobInfo.Title,
                    FirstName = CacheManager.JobInfo.FirstName,
                    LastName = CacheManager.JobInfo.LastName,
                    PositionApplied = CacheManager.JobInfo.PositionApplied,
                    KnownAs = CacheManager.JobInfo.KnownAs,
                    ChineseCharacter = CacheManager.JobInfo.ChineseCharacter,
                    Gender = CacheManager.JobInfo.Gender,
                    DateOfBirth = CacheManager.JobInfo.DateOfBirth,
                    CountryOfBirth = CacheManager.JobInfo.CountryOfBirth,
                    Nationality = CacheManager.JobInfo.Nationality,
                    Race = CacheManager.JobInfo.Race,
                    MaritalStatus = CacheManager.JobInfo.MaritalStatus,
                    Religion = CacheManager.JobInfo.Religion,
                    IdentityNo = CacheManager.JobInfo.IdentityNo,
                    DateOfIssue = CacheManager.JobInfo.DateOfIssue,
                    DateOfExpiry = CacheManager.JobInfo.DateOfExpiry,
                    CountryOfIssue = CacheManager.JobInfo.CountryOfIssue,
                    EmailAddress = CacheManager.JobInfo.EmailAddress
                };

                Database.UpdateJobApplication(model);
            }
        }
    }
}