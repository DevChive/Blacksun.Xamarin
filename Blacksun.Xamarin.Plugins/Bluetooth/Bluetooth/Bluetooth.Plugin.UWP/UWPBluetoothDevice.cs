using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using WindowsBluetooth;
using Plugin.Bluetooth.Abstractions;
using Plugin.Bluetooth.Abstractions.Exceptions;

namespace Bluetooth.Plugin.UWP
{
    public class UWPBluetoothDevice : IBluetoothDevice
    {
        /// <summary>
        /// DataWriter used to send commands easily.
        /// </summary>
        private DataWriter dataWriter;

        /// <summary>
        /// DataReader used to receive messages easily.
        /// </summary>
        private DataReader dataReader;

        private Guid CurrentUniqueIdentifier { get; set; }

        StreamSocket _socket { get; set; }

        public DeviceInformation BluetoothDevice { get; set; }

        private RfcommDeviceService _service;

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


        public bool ContainsUniqueIdentifier(string uniqueIdentifier)
        {
            return false;
        }

        public void SetUniqueIdentifier(string uniqueIdentifier)
        {

        }

        public bool ContainsUniqueIdentifier(Guid uniqueIdentifier)
        {
            return false;
        }

        public void SetUniqueIdentifier(Guid uniqueIdentifier)
        {

        }

        public async Task Connect()
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                _socket = new StreamSocket();
                _service = await RfcommDeviceService.FromIdAsync(BluetoothDevice.Id);
                await _socket.ConnectAsync(_service.ConnectionHostName, _service.ConnectionServiceName);
                dataReader = new DataReader(_socket.InputStream);
                dataWriter = new DataWriter(_socket.OutputStream);
                IsConnected = true;

            }
            catch (Exception ex)
            {
                Disconnect();
                throw new BluetoothDeviceNotFoundException("Couldnt connect to " + Name);
            }


        }


        public async Task Disconnect()
        {

            try
            {
                await _socket.CancelIOAsync();
                _socket.Dispose();
                _socket = null;
                _service.Dispose();
                _service = null;
                
            }
            catch (Exception ex)
            {
                
            }
            IsConnected = false;

        }

        public async Task Write(string message)
        {
            dataWriter.WriteString(message);
            await dataWriter.StoreAsync();

        }

        public async Task Write(byte[] bytes)
        {
            dataWriter.WriteBytes(bytes);
            await dataWriter.StoreAsync();

        }
    }
}
