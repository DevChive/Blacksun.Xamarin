using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Bluetooth.Abstractions
{
    public interface IBluetoothDevice
    {
        List<Guid> UniqueIdentifiers { get; }

        string Name { get; set; }

        string Address { get; set; }

        bool IsConnected { get; set; }

        Task Connect();

        Task Connect(int port);

        Task Disconnect();

        Task Write(string message);

        Task Write(byte[] bytes);
    }
}
