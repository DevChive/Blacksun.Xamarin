using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Bluetooth.Plugin.WindowsPhone81;
using Plugin.Bluetooth.Abstractions;

namespace Plugin.Bluetooth
{
    public class BluetoothImplementation : BaseBluetooth
    {

        public override bool IsAvailable
        {
            get
            {

                try
                {

                    Windows.Networking.Proximity.PeerFinder.Start();

                    var task = Windows.Networking.Proximity.PeerFinder.FindAllPeersAsync().AsTask();
                    task.Wait();
                    
                    return true; //boolean variable
                }
                catch (Exception ex)
                {
                    if ((uint)ex.HResult == 0x8007048F)
                    {
                        return false;
                    }

                }


                return false;
            }
        }

        public override bool IsTurnedOn
        {
            get
            {
                try
                {

                    Windows.Networking.Proximity.PeerFinder.Start();

                    var task = Windows.Networking.Proximity.PeerFinder.FindAllPeersAsync().AsTask();
                    task.Wait();

                    return true; //boolean variable
                }
                catch (Exception ex)
                {
                    if ((uint)ex.HResult == 0x8007048F)
                    {
                        return false;
                    }

                }


                return false;
            }
        }

        public override async Task<List<IBluetoothDevice>> GetPairedDevices()
        {
            var devices = new List<IBluetoothDevice>();

            var selector = BluetoothDevice.GetDeviceSelector();

            DeviceInformationCollection DeviceInfoCollection
                = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));

            //var pairedDevices = await DeviceInformation.FindAllAsync(selector);

            foreach (var deviceInfo in DeviceInfoCollection)
            {

                //var _service = await RfcommDeviceService.FromIdAsync(deviceInfo.Id);

                var device = new WindowsPhone81BluetoothDevice(){ Name = deviceInfo.Name, Address = string.Empty };

                device.BluetoothDevice = deviceInfo;


                var id = string.Empty;
                if (deviceInfo.Id.Contains("BTHENUM#{"))
                {
                    var index = deviceInfo.Id.IndexOf("BTHENUM#{")+9;
                    id = deviceInfo.Id.Substring(index, 36);
                }

                if (!String.IsNullOrEmpty(id))
                {
                    var guid = Guid.Parse(id);
                    device.UniqueIdentifiers.Add(guid);
                }

                devices.Add(device);
                
            }

            return devices;

        }

        public override Task<List<IBluetoothDevice>> FindDevicesWithIdentifier(string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
