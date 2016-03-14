using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using Plugin.Bluetooth.Abstractions;
using System.IO;

namespace Bluetooth.Plugin.Android
{
    public class AndroidBluetoothDevice : IBluetoothDevice
    {

        private static UUID MY_UUID = UUID.FromString("fa87c0d0-afac-11de-8a39-0800200c9a66");
        private BluetoothSocket Socket = null;
        private Guid CurrentUniqueIdentifier { get; set; }

        private readonly List<Guid> _uniqueIdentifiers = new List<Guid>();
        public List<Guid> UniqueIdentifiers
        {
            get { return _uniqueIdentifiers; }
        }

        public string Name { get; set; }
        public string Address { get; set; }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }

        public BluetoothDeviceType Type { get; set; }
        public BluetoothDevice BluetoothDevice { get; set; }
        private Stream outStream = null;
        private Stream inStream = null;
        public void SetUniqueIdentifier(Guid uniqueIdentifier)
        {
            CurrentUniqueIdentifier = uniqueIdentifier;
        }

        public async Task Connect()
        {
            await Connect(1);
        }

        public Task Connect(int port)
        {

            var tc = new TaskCompletionSource<bool>();

            var connThread = new ConnectThread(BluetoothDevice, port);

            connThread.DeviceConnected += (o, t) =>
            {
                IsConnected = true;
                Socket = connThread.mmSocket;
                inStream = Socket.InputStream;
                outStream = Socket.OutputStream;
                tc.TrySetResult(true);
            };

            connThread.DeviceConnectionFailed += (o, t) =>
            {
                IsConnected = false;
                tc.TrySetResult(false);
            };

            connThread.Run();

            return tc.Task;

        }

        public async Task Disconnect()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (inStream != null)
                    {
                        try
                        {
                            inStream.Close();
                        }
                        catch (Exception e)
                        {
                        }
                        inStream = null;
                    }

                    if (outStream != null)
                    {
                        try
                        {
                            outStream.Close();
                        }
                        catch (Exception e)
                        {
                        }
                        outStream = null;
                    }

                    if (Socket != null)
                    {
                        try
                        {
                            Socket.Close();
                        }
                        catch (Exception e)
                        {
                        }
                        Socket = null;
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    IsConnected = false;
                }
            });
        }


        public async Task Write(string message)
        {
            await Write(Encoding.UTF8.GetBytes(message));
        }

        public async Task Write(byte[] bytes)
        {
            try
            {
                outStream = Socket.OutputStream;
            }
            catch (IOException ex)
            {
                throw new System.IO.IOException(ex.Message);
            }

            var msgBuffer = bytes;
            try
            {
                outStream.Write(msgBuffer, 0, msgBuffer.Length);
            }
            catch (IOException ex)
            {
                throw new System.IO.IOException(ex.Message);
            }
        }


        public bool ContainsUniqueIdentifier(string uniqueIdentifier)
        {
            return ContainsUniqueIdentifier(Guid.Parse(uniqueIdentifier));
        }

        public void SetUniqueIdentifier(string uniqueIdentifier)
        {
            SetUniqueIdentifier(Guid.Parse(uniqueIdentifier));
        }

        public bool ContainsUniqueIdentifier(Guid uniqueIdentifier)
        {
            var uuids = BluetoothDevice.GetUuids().ToList();

            foreach (var uuid in uuids)
            {
                var stringUUID = uuid.ToString();
                if (stringUUID == uniqueIdentifier.ToString())
                {
                    return true;
                }
            }


            return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}