using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace hlin_v01
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class savedKeys : Page
    {
        public savedKeys()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("keys.ini");
                List<keys> items = new List<keys>();
                string temp = await Windows.Storage.FileIO.ReadTextAsync(file);
                string[] arr = temp.Split('\t');

                foreach (string s in arr)
                {
                    string[] arra = s.Split('|');
                    int index = s.IndexOf(arra[0]);
                    string endOfString = s.Substring(index + arra[0].Length);
                    string[] arrb = endOfString.Split('\b');
                    index = s.IndexOf(arrb[0]);
                    endOfString = s.Substring(index + arrb[0].Length);
                    string[] arrc = endOfString.Split(';');
                    keys obj = new keys(arra[0], arrb[0], arrc[0]);
                    items.Add(obj);
                }
                list.ItemsSource = items;
            }
            catch
            {

            }
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
            e.Handled = true;
        }
        private async void popup(string pop, string title)
        {
            var messageDialog = new MessageDialog(pop);
            messageDialog.Title = title;
            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand(
                "Close",
                new UICommandInvokedHandler(this.CommandInvokedHandler)));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }
        private void CommandInvokedHandler(IUICommand command)
        {
            // Display message showing the label of the command that was invoked

        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            string title = "Version:";
            string pop = "The Current Version: HLIN_v4.0.0.0 \nDeveloped by: CyberKnights \nPackage: Final version";
            popup(pop, title);

        }
        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            string title = "About:";
            string pop = "HLIN is an application which mainly focuses on Secured one-to-many file sharing even without the presence of the sender while receiving.";
            popup(pop, title);
        }
        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(feedback));
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void list_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (keys)e.ClickedItem;
            Frame.Navigate(typeof(entry), clickedItem.userid);
        }
    }
}
