using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Bluetooth;
using Plugin.Bluetooth.Abstractions;

namespace BluetoothTest.Android
{
    [Activity(Label = "BluetoothTest.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        List<IBluetoothDevice> items;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            items = new List<IBluetoothDevice>();

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton); 
            Button getButton = FindViewById<Button>(Resource.Id.GetButton); 
                
              
            TextView textView1 = FindViewById<TextView>(Resource.Id.textView1);
            ListView listview = FindViewById<ListView>(Resource.Id.list);
            var adapter = new CustomAdapter(this,items);
            listview.Adapter = adapter;
            listview.ItemClick += async (o, t) =>
            {
                if (t.Position != -1)
                {
                    var item = adapter[t.Position];
                    listview.ClearChoices();
                    if (item == null)
                        return;
                    try
                    {
                        await item.Connect();
                        if (item.IsConnected)
                        {
                            await item.Write("Lorem ipsum dolorem");
                            await item.Disconnect();
                        }
                        else
                        {
                            ShowAlert("No se pudo conectar con el dispositivo");
                        }

                    }
                    catch (Exception ex)
                    {
                        ShowAlert(ex.Message);

                    }
                }
                
            };

            button.Click += delegate
            {
                using (var bluetooth = CrossBluetooth.Current)
                {
                    textView1.Text = bluetooth.IsTurnedOn ? "On" : "Off";
                }
            };

            getButton.Click += async(o, t) =>
            {
                using (var bluetooth = CrossBluetooth.Current)
                {
                    adapter.Clear();
                    foreach (var device in await bluetooth.GetPairedDevices())
                    {
                        adapter.Add(device);
                    }

                }
            };

        }

        public void ShowAlert(string str)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(str);
            alert.SetPositiveButton("OK", (senderAlert, args) => {
                // write your own set of instructions
            });

            //run the alert in UI thread to display in the screen
            RunOnUiThread(() => {
                alert.Show();
            });
        }
    }
}

