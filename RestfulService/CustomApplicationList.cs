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
using MyAverisEntity;

namespace MyAveris.Droid
{
    public class CustomApplicationList : BaseAdapter<ShortJobApplicationInfo>
    {
        List<ShortJobApplicationInfo> items;
        Activity context;
        public CustomApplicationList(Activity context, List<ShortJobApplicationInfo> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override ShortJobApplicationInfo this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.CustomApplicationList, null);
            view.FindViewById<TextView>(Resource.Id.NameText).Text = item.FullName;
            view.FindViewById<TextView>(Resource.Id.PositionText).Text = item.PositionApplied;
            view.FindViewById<TextView>(Resource.Id.StatusText).Text = item.ApplicationStatus;
            return view;
        }
    }
}