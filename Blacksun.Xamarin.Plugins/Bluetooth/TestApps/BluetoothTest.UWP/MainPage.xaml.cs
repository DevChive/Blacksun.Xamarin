using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Plugin.Bluetooth;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BluetoothTest.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var bluetooth = CrossBluetooth.Current)
            {
                IsOnLabel.Text = bluetooth.IsTurnedOn ? "On" : "Off";
            }

        }

        private async void Button_PairedDevicesClick(object sender, RoutedEventArgs e)
        {
            using (var bluetooth = CrossBluetooth.Current)
            {
                listView.ItemsSource = await bluetooth.GetPairedDevices();
            }
        }
    }
}
