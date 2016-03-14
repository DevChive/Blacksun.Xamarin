using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.Bluetooth.Abstractions;

namespace BluetoothTest.Android
{
    public class CustomAdapter : BaseAdapter<IBluetoothDevice>
    {
        List<IBluetoothDevice> items;
        Activity context;
        public CustomAdapter(Activity context, List<IBluetoothDevice> items) : base() {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override IBluetoothDevice this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }

        public void Add(IBluetoothDevice device)
        {
            items.Add(device);
            this.NotifyDataSetChanged();
        }

        public void Clear()
        {
            items.Clear();
            this.NotifyDataSetChanged();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(global::Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(global::Android.Resource.Id.Text1).Text = items[position].Name;
            return view;
        }
    }
}