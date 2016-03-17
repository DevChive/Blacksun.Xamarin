using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Plugin.Bluetooth;
using Plugin.Bluetooth.Abstractions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BluetoothTestWindowsPhone81
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
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
                progressRing.IsActive = true;
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
                    progressRing.IsActive = false;
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
