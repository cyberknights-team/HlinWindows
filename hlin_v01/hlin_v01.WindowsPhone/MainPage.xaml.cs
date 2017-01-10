using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace hlin_v01
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

        private void lock_1_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(lock_form));
        }
        private void receive_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(entry));
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

        private void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(savedKeys));
        }
    }
}
