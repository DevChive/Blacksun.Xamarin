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
using Java.IO;
using Java.Lang;
using Plugin.Bluetooth.Abstractions.Exceptions;
using Exception = System.Exception;
using Java.Util;

namespace Bluetooth.Plugin.Android
{
    public class ConnectThread : Thread
    {

        public BluetoothSocket mmSocket;
        private BluetoothDevice Device;
        private BluetoothAdapter mBluetoothAdapter;
        private int Port = 1;

        public ConnectThread(BluetoothDevice device)
        {
            // Use a temporary object that is later assigned to mmSocket,
            // because mmSocket is final
            BluetoothSocket tmp = null;
            Device = device;
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (mBluetoothAdapter == null)
            {
                // Device does not support Bluetooth
                throw new System.Exception("No bluetooth device found");
            }


        }

        public override void Run()
        {

            try
            {

                ProbeConnection();


            }
            catch (IOException connectException)
            {

                // Unable to connect; close the socket and get out
                try
                {
                    mmSocket.Close();
                }
                catch (IOException closeException)
                {
                    
                }
                throw new BluetoothDeviceNotFoundException(connectException.Message);
                return;
            }



            //base.Run();

        }

        private void ProbeConnection()
        {
            ParcelUuid[] parcelUuids;
            parcelUuids = Device.GetUuids();
            bool isConnected = false;



            if (parcelUuids == null)
            {
                parcelUuids = new ParcelUuid[1];
                parcelUuids[0] = new ParcelUuid(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            }

            foreach (var parcelUuid in parcelUuids)
            {
                mBluetoothAdapter.CancelDiscovery();
                //METHOD A

                try
                {
                    var method = Device.GetType().GetMethod("createRfcommSocket");
                    mmSocket = (BluetoothSocket)method.Invoke(Device, new object[] { Port });
                    mmSocket.Connect();
                    isConnected = true;
                    DoDeviceConnected();
                    break;
                }
                catch (Exception)
                {

                }

                //METHOD B

                try
                {
                    var method = Device.GetType().GetMethod("createInsecureRfcommSocket");
                    mmSocket = (BluetoothSocket)method.Invoke(Device, new object[] { Port });
                    mmSocket.Connect();
                    isConnected = true;
                    DoDeviceConnected();
                    break;
                }
                catch (Exception)
                {

                }

            }

            if (!isConnected)
            {
                //METHOD C

                try
                {
                    IntPtr createRfcommSocket = JNIEnv.GetMethodID(Device.Class.Handle, "createRfcommSocket", "(I)Landroid/bluetooth/BluetoothSocket;");
                    IntPtr _socket = JNIEnv.CallObjectMethod(Device.Handle, createRfcommSocket, new global::Android.Runtime.JValue(Port));
                    mmSocket = Java.Lang.Object.GetObject<BluetoothSocket>(_socket, JniHandleOwnership.TransferLocalRef);

                    mmSocket.Connect();
                    DoDeviceConnected();
                }
                catch (IOException connectException)
                {

                    // Unable to connect; close the socket and get out
                    try
                    {
                        mmSocket.Close();
                    }
                    catch (IOException closeException)
                    {

                    }
                    DoDeviceConnectionFailed();
                }
            }
        }

        public event EventHandler DeviceConnected;
        private void DoDeviceConnected()
        {

            if (DeviceConnected != null)
            {
                DeviceConnected(this, new EventArgs());
            }
        }

        public event EventHandler DeviceConnectionFailed;
        private void DoDeviceConnectionFailed()
        {
            if (DeviceConnectionFailed != null)
            {
                DeviceConnectionFailed(this, new EventArgs());
            }
        }

        /** Will cancel an in-progress connection, and close the socket */
        public void Cancel()
        {
            try
            {
                mmSocket.Close();
            }
            catch (IOException e) { }
        }


    }
}