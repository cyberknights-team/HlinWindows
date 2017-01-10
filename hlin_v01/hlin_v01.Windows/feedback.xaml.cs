using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
    public sealed partial class feedback : Page
    {
        public feedback()
        {
            this.InitializeComponent();
        }
        StorageFile file;
        config connection = new config();
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(feedback));
        }

        private async void send_Click(object sender, RoutedEventArgs e)
        {
            if (Feedback.Text != "")
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    show_progress();
                    invisible();
                    loading_text.Text = "Connecting to the server ...";
                    string key = connection.get_key();
                    string tableName = "feedback";
                    string containerName = "feedback";
                    string counter = await get_counter(key, containerName);
                    temp_obj temp = new temp_obj(Feedback.Text.ToString(),"Public",counter);
                    try
                    {
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

                        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                        CloudTable table = tableClient.GetTableReference(tableName);
                        loading_text.Text = "Sending your Feedback ...";
                        await table.CreateIfNotExistsAsync();

                        TableOperation insertOperation = TableOperation.InsertOrReplace(temp);

                        await table.ExecuteAsync(insertOperation);

                        if (file!=null)
                        {
                            await attach_picture(key, "feedback", counter);
                        }
                        
                        loading_text.Text = "Completed ...";
                        hide_progress();
                        visible();
                        Feedback.Text = "";
                        popup("Thank you for your suggestion \nWe'll get back to you shortly. :)", "Message:");
                        
                    }
                    catch
                    {

                        popup("Error in entering table", "Error:");
                        hide_progress();
                        visible();
                    }

                }
                else
                {
                    popup("WHOOPS! you are not connected to the internet. \nPlease check your internet connectionand try again", "Error:");
                    hide_progress();
                    visible();
                }
            }
            else
            {
                popup("Please enter the text ...","Error:");
                hide_progress();
                visible();
            }

        }
        private async Task<string> get_counter(string key, string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            CloudBlockBlob counterBlob = container.GetBlockBlobReference("counter_feedback");

            string content = await counterBlob.DownloadTextAsync();
            int number = Int32.Parse(content) + 1;
            if(number==100)
            {
                number = 0;
            }
            await update_counter(key, containerName, number.ToString());
            return content;
        }
        private async Task update_counter(string key, string containerName, string rowKey)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            CloudBlockBlob counterBlob = container.GetBlockBlobReference("counter_feedback");
            await counterBlob.UploadTextAsync(rowKey);
        }
        private async Task attach_picture(string key,string containerName,string rowkey)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob counterBlob = container.GetBlockBlobReference(rowkey);

            await counterBlob.UploadFromFileAsync(file);
        }

        private void show_progress()
        {
            loading.IsActive = true;
            loading.Visibility = Visibility.Visible;
            loading_text.Visibility = Visibility.Visible;
        }
        private void hide_progress()
        {
            loading.IsActive = false;
            loading.Visibility = Visibility.Collapsed;
            loading_text.Visibility = Visibility.Collapsed;
        }
        private void invisible()
        {
            Feedback.Visibility = Visibility.Collapsed;
            send.Visibility = Visibility.Collapsed;
            title.Visibility = Visibility.Collapsed;
            Attach.Visibility = Visibility.Collapsed;
            nameOfFile.Text = "";
        }
        private void visible()
        {
            Feedback.Visibility = Visibility.Visible;
            send.Visibility = Visibility.Visible;
            title.Visibility = Visibility.Visible;
            Attach.Visibility = Visibility.Visible;
        }

        private async void Attach_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileOpenPicker picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".bmp");
                file = await picker.PickSingleFileAsync();
                nameOfFile.Text = file.Name.ToString();

            }
            catch
            {
                popup("Please select a file ... ", "Error:");
            }
        }
    }
}
