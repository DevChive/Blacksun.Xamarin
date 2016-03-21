using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Plugin.Bluetooth;
using Plugin.Bluetooth.Abstractions;

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

        private async void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (listView.SelectedItem != null)
            {
                var item = listView.SelectedItem as IBluetoothDevice;
                listView.SelectedItem = null;
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
                finally
                {
                }
                
                
            }

        }

        public void ShowAlert(string str)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(
                            str,
                            "Error");
            dialog.ShowAsync();
        }

    }
}
