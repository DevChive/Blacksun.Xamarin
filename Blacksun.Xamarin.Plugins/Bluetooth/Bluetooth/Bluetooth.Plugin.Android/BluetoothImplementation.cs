using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bluetooth.Plugin.Android;
using Plugin.Bluetooth.Abstractions;

namespace Plugin.Bluetooth
{
    public class BluetoothImplementation : BaseBluetooth
    {

        private BluetoothAdapter btAdapter;
        private CustomBroadcastReceiver broadcastReceiver;

        public BluetoothImplementation()
        {
            btAdapter = BluetoothAdapter.DefaultAdapter;
        }

        public override bool IsAvailable
        {
            get { return true; }
        }

        public override bool IsTurnedOn
        {
            get
            {
                var bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                if (bluetoothAdapter == null)
                {
                    // Device does not support Bluetooth
                    throw new Exception("Device does not support Bluetooth");
                }
                else
                {
                    return bluetoothAdapter.IsEnabled;
                }
            }
        }

        public override async Task<List<IBluetoothDevice>> GetPairedDevices()
        {

            var devices = new List<IBluetoothDevice>();

            try
            {
                // Get a set of currently paired devices 
                var pairedDevices = btAdapter.BondedDevices;

                if (btAdapter.BondedDevices == null)
                {
                    
                }
                else
                {
                    // If there are paired devices, add each one to the ArrayAdapter 
                    if (pairedDevices.Count > 0)
                    {

                        foreach (var paireddevice in pairedDevices)
                        {
                            var device = new AndroidBluetoothDevice() { Name = paireddevice.Name, Address = paireddevice.Address };
                            device.BluetoothDevice = paireddevice;
                            try
                            {
                                switch (paireddevice.Type)
                                {
                                    case global::Android.Bluetooth.BluetoothDeviceType.Classic:
                                        device.Type = BluetoothDeviceType.Classic;
                                        break;
                                    case global::Android.Bluetooth.BluetoothDeviceType.Dual:
                                        device.Type = BluetoothDeviceType.Dual;
                                        break;
                                    case global::Android.Bluetooth.BluetoothDeviceType.Le:
                                        device.Type = BluetoothDeviceType.Le;
                                        break;
                                    case global::Android.Bluetooth.BluetoothDeviceType.Unknown:
                                        device.Type = BluetoothDeviceType.Unknown;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                device.BluetoothDevice.FetchUuidsWithSdp();

                                var array = paireddevice.GetUuids();
                                if (array != null)
                                {
                                    var uuids = array.ToList();

                                    foreach (var uuid in uuids)
                                    {
                                        System.Diagnostics.Debug.WriteLine(device.Name + " Adding uuid " + uuid.Uuid.ToString());
                                        var stringUUID = uuid.ToString();
                                        device.UniqueIdentifiers.Add(Guid.Parse(stringUUID));
                                    }
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Bluetooth Android: No Paired devices");
                                }


                            }
                            catch (Java.Lang.Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex.Message);
                            }

                            devices.Add(device);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Bluetooth Android: No devices found");
                    }
                }


            }
            catch (Java.Lang.Exception ex)
            {
                throw new Exception(ex.Message);
            }


            return devices;

        }

        public override Task<List<IBluetoothDevice>> FindDevicesWithIdentifier(string identifier)
        {
            throw new NotImplementedException();
        }

    }
}