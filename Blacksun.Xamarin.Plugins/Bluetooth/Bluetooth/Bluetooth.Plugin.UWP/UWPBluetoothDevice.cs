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

        StreamSocket Socket { get; set; }

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


        public UWPBluetoothDevice()
        {
            Socket = new StreamSocket();
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
            await Connect(1);
        }

        public async Task Connect(int port)
        {
            
            try
            {
                _service = await RfcommDeviceService.FromIdAsync(BluetoothDevice.Id);
                Socket = new StreamSocket();
                await Socket.ConnectAsync(_service.ConnectionHostName,_service.ConnectionServiceName);
                dataReader = new DataReader(Socket.InputStream);
                dataWriter = new DataWriter(Socket.OutputStream);
                IsConnected = true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                throw new BluetoothDeviceNotFoundException("Couldnt find " + Name);
            }
            
        }

        public async Task Disconnect()
        {
            Socket.Dispose();
            dataReader = null;
            dataWriter = null;

            await Task.Delay(2000);

        }

        public async Task Write(string message)
        {
            uint sentCommandSize = 0;

            uint commandSize = dataWriter.MeasureString(message);
            dataWriter.WriteByte((byte)commandSize);
            sentCommandSize = dataWriter.WriteString(message);
            await dataWriter.StoreAsync();
        }

        public async Task Write(byte[] bytes)
        {

            var str = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            await Write(str);

        }
    }
}
