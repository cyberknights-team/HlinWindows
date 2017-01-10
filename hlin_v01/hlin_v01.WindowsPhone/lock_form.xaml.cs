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
using Windows.Devices.Geolocation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Windows.UI.Popups;
using Windows.Data.Xml.Dom;
using System.Xml.Serialization;
using System.Xml;
using System.Threading;
using System.Net.NetworkInformation;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using Windows.Phone.UI.Input;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Activation;




// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace hlin_v01
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class lock_form : Page
    {


        public lock_form()
        {
            this.InitializeComponent();

        }
        string error = "";
        string userkey = "";
        string type;
        string title = "Error:";
        string ssid = "";
        StorageFile file;
        CoreApplicationView view = CoreApplication.GetCurrentView();
        config connection = new config();
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            password_check.IsChecked = true;
            latitude.Visibility = Visibility.Collapsed;
            longitude.Visibility = Visibility.Collapsed;
            getLocationCheck.Visibility = Visibility.Collapsed;
            save_key.Visibility = Visibility.Collapsed;
            remember.Visibility = Visibility.Collapsed;
            slider.Visibility = Visibility.Collapsed;
            slider_text.Visibility = Visibility.Collapsed;
            SSID_text.Visibility = Visibility.Collapsed;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
            e.Handled = true;
        }
        
        private async void getLocationCheck_Checked(object sender, RoutedEventArgs e)
        {

            show_progress();
            loading_text.Text = "Fetching location...";
            invisible();
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracy = Windows.Devices.Geolocation.PositionAccuracy.High;
            geolocator.DesiredAccuracyInMeters = 50;
            try
            {
                //Timer timer = new Timer(1000);


                Geoposition geoposition = await geolocator.GetGeopositionAsync(TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1));
                latitude.Text = geoposition.Coordinate.Point.Position.Latitude.ToString("0.0000000000");
                longitude.Text = geoposition.Coordinate.Point.Position.Longitude.ToString("0.0000000000");
                loading_text.Text = "Location has been pinned...";
                hide_progress();
                visible();

            }
            catch
            {
                error += "Error in fetching latitude...\n Try again...";
            }
        }

        private void getLocationCheck_Unchecked(object sender, RoutedEventArgs e)
        {

            longitude.Text = "";
            latitude.Text = "";
            hide_progress();
        }

        private async void upload_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                string verify1 = "", verify2 = "";
                if (type == "1")
                {
                    verify1 += password.Text.ToString();
                    verify2 += "yes";
                }
                else if (type == "2")
                {
                    verify1 += latitude.Text.ToString();
                    verify2 += longitude.Text.ToString();
                }
                else
                {
                    verify1 = "yes";
                    verify2 = "yes";
                }
                if ((verify1 != "") && (verify2 != ""))
                {
                    if (type != "")
                    {
                        FileOpenPicker openPicker = new FileOpenPicker();
                        openPicker.ViewMode = PickerViewMode.Thumbnail;
                        openPicker.FileTypeFilter.Add("*");
                        try
                        {
                            file = await openPicker.PickSingleFileAsync();
                            var properties = await file.GetBasicPropertiesAsync();
                            double size = 5.0 * 10 * 10 * 10 * 10 * 10 * 10 * 10;

                            if (file != null)
                            {
                                if (properties.Size < size)
                                {
                                    invisible();
                                    show_progress();
                                    loading_text.Text = "Connecting to server...";

                                    userkey = await generate_upload(file);
                                    hide_progress();
                                    display_result();



                                }
                                else
                                {
                                    error += "The file is too big";
                                    popup();
                                }
                            }
                        }
                        catch
                        {
                            
                            openPicker.PickSingleFileAndContinue();
                            view.Activated += viewActivated;
                            var properties = await file.GetBasicPropertiesAsync();
                            double size = 5.0 * 10 * 10 * 10 * 10 * 10 * 10 * 10;

                            if (file != null)
                            {
                                if (properties.Size < size)
                                {
                                    invisible();
                                    show_progress();
                                    loading_text.Text = "Connecting to server...";

                                    userkey = await generate_upload(file);
                                    hide_progress();
                                    display_result();



                                }
                                else
                                {
                                    error += "The file is too big";
                                    popup();
                                }
                            }
                        }
                        
             
                          
                        
                       


                    }
                    else
                    {
                        error += "Please select a option...";
                        popup();
                    }
                }
                else
                {
                    error += "Please enter ";
                    if (type == "1")
                    {
                        error += "Password to continue";
                    }
                    else
                    {
                        error += "location co-ordinates to continue";
                    }
                    popup();
                }
            }
            else
            {
                error += "Oops! You are not connected to the internet! \nPlease check your internet connection and try again...";
                popup();
            }
        }

        private void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                view.Activated -= viewActivated;
                file = args.Files[0];

            }
        }

        private async void popup()
        {
            var messageDialog = new MessageDialog(error);
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
            error = "";
            title = "Error:";
        }

        private async Task save_keys(string blob)
        {

            string content = userkey + "|" + remember.Text.ToString() + "\b" + DateTime.Now.ToString() + ";" + "\t";
            StorageFile tempfile;
            try
            {
                tempfile = await ApplicationData.Current.LocalFolder.CreateFileAsync("keys.ini", CreationCollisionOption.FailIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(tempfile, content);
            }
            catch
            {
                tempfile = await ApplicationData.Current.LocalFolder.GetFileAsync("keys.ini");
                await Windows.Storage.FileIO.AppendTextAsync(tempfile, content);
            }
        }



        private async Task<string> generate_upload(StorageFile file)
        {
            string key = connection.get_key();
            string containerName = "public";
            string blob = "HLIN";
            string tableName = "users";


            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            CloudBlockBlob counterBlob = container.GetBlockBlobReference("counter");

            string content = await counterBlob.DownloadTextAsync();


            int number = Int32.Parse(content) + 1;
            string rowkey = (number.ToString());
            blob += type;
            blob += rowkey;
            loading_text.Text = "Uploading ...";
            await create_blob(key, containerName, blob, file);
            storage item = new storage("Public", rowkey, password.Text.ToString(), latitude.Text.ToString(), longitude.Text.ToString(), containerName, file.FileType.ToString(), blob, DateTime.Now, slider.Value, ssid);
            loading_text.Text = "Finalising ...";
            create_table(key, tableName, item);

            await update_counter(key, containerName, rowkey);
            loading_text.Text = "Upload done";
            return blob;
        }





        private async Task create_blob(string key, string containerName, string blob, StorageFile file)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                await container.CreateIfNotExistsAsync();
                //  File.WriteAllBytes(filename, Convert.FromBase64String(content));
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blob);
                await blockBlob.UploadFromFileAsync(file);

            }
            catch
            {
                error += "Error in uploading file";
                popup();

            }
        }


        private async void create_table(string key, string tableName, storage item)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                CloudTable table = tableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();

                TableOperation insertOperation = TableOperation.InsertOrReplace(item);

                await table.ExecuteAsync(insertOperation);
            }
            catch
            {
                error += "Error in Entering table...\nPlease Try again.";
                popup();
            }
        }

        private async Task update_counter(string key, string containerName, string rowKey)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            CloudBlockBlob counterBlob = container.GetBlockBlobReference("counter");
            await counterBlob.UploadTextAsync(rowKey);

        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void none_Checked(object sender, RoutedEventArgs e)
        {

            password.Visibility = Visibility.Collapsed;
            latitude.Visibility = Visibility.Collapsed;
            longitude.Visibility = Visibility.Collapsed;
            getLocationCheck.Visibility = Visibility.Collapsed;
            slider.Visibility = Visibility.Collapsed;
            slider_text.Visibility = Visibility.Collapsed;
            SSID_text.Visibility = Visibility.Collapsed;
            type = "0";

        }

        private void password_check_Checked(object sender, RoutedEventArgs e)
        {
            password.Visibility = Visibility.Visible;
            latitude.Visibility = Visibility.Collapsed;
            longitude.Visibility = Visibility.Collapsed;
            getLocationCheck.Visibility = Visibility.Collapsed;
            slider.Visibility = Visibility.Collapsed;
            slider_text.Visibility = Visibility.Collapsed;
            SSID_text.Visibility = Visibility.Collapsed;
            type = "1";
        }

        private void geolock_Checked(object sender, RoutedEventArgs e)
        {
            password.Visibility = Visibility.Collapsed;
            latitude.Visibility = Visibility.Visible;
            longitude.Visibility = Visibility.Visible;
            getLocationCheck.Visibility = Visibility.Visible;
            slider.Visibility = Visibility.Visible;
            slider_text.Visibility = Visibility.Visible;
            SSID_text.Visibility = Visibility.Collapsed;
            type = "2";
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            title = "Version:";
            error += "The Current Version: HLIN_v4.0.0.0 \nDeveloped by: CyberKnights \nPackage: Final version";
            popup();

        }
        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            title = "About:";
            error += "HLIN is an application which mainly focuses on Secured one-to-many file sharing, even without the presence of the sender while receiving.";
            popup();
        }
        private void invisible()
        {
            password.Visibility = Visibility.Collapsed;
            latitude.Visibility = Visibility.Collapsed;
            longitude.Visibility = Visibility.Collapsed;
            upload.Visibility = Visibility.Collapsed;
            getLocationCheck.Visibility = Visibility.Collapsed;
            none.Visibility = Visibility.Collapsed;
            password_check.Visibility = Visibility.Collapsed;
            geolock.Visibility = Visibility.Collapsed;
            selection.Visibility = Visibility.Collapsed;
            uploadMultiple.Visibility = Visibility.Collapsed;
            slider.Visibility = Visibility.Collapsed;
            slider_text.Visibility = Visibility.Collapsed;
            network_lock.Visibility = Visibility.Collapsed;
            SSID_text.Visibility = Visibility.Collapsed;
        }

        private void visible()
        {
            latitude.Visibility = Visibility.Visible;
            longitude.Visibility = Visibility.Visible;
            upload.Visibility = Visibility.Visible;
            getLocationCheck.Visibility = Visibility.Visible;
            none.Visibility = Visibility.Visible;
            password_check.Visibility = Visibility.Visible;
            geolock.Visibility = Visibility.Visible;
            selection.Visibility = Visibility.Visible;
            uploadMultiple.Visibility = Visibility.Visible;
            slider.Visibility = Visibility.Visible;
            slider_text.Visibility = Visibility.Visible;
            network_lock.Visibility = Visibility.Visible;

        }
        private void display_result()
        {
            result.Text = "Your file has been uploaded.\nYour Access Key is:" + userkey;
            remember.Visibility = Visibility.Visible;
            save_key.Visibility = Visibility.Visible;
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
        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(feedback));
        }

        private async void save_key_Click(object sender, RoutedEventArgs e)
        {
            await save_keys(userkey);
            title = "Message:";
            error += "Your Access Key has been saved...";
            popup();
            Frame.Navigate(typeof(MainPage));
        }

        private async void uploadMultiple_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                string verify1 = "", verify2 = "";
                if (type == "1")
                {
                    verify1 += password.Text.ToString();
                    verify2 += "yes";
                }
                else if (type == "2")
                {
                    verify1 += latitude.Text.ToString();
                    verify2 += longitude.Text.ToString();
                }
                else
                {
                    verify1 = "yes";
                    verify2 = "yes";
                }
                if ((verify1 != "") && (verify2 != ""))
                {
                    if (type != "")
                    {
                        FileOpenPicker openPicker = new FileOpenPicker();
                        openPicker.ViewMode = PickerViewMode.Thumbnail;
                        openPicker.FileTypeFilter.Add("*");

                        IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
                        if (files.Count <= 10)
                        {
                            bool dec = true;
                            foreach (StorageFile file in files)
                            {
                                var properties = await file.GetBasicPropertiesAsync();
                                double size = 5.0 * 10 * 10 * 10 * 10 * 10 * 10 * 10;
                                if (properties.Size > size)
                                {
                                    error = "Every file has to be less than 50mb..";
                                    popup();
                                    dec = false;
                                    break;
                                }
                            }
                            if (dec)
                            {
                                try
                                {
                                    invisible();
                                    show_progress();
                                    loading_text.Text = "Connecting to server...";
                                    userkey = await generate_upload_multiple(files);
                                    hide_progress();
                                    display_result();
                                }
                                catch
                                {
                                    error += "Please select a file...";
                                    popup();
                                }
                            }
                        }
                        else
                        {
                            error += "Only 10 files are allowed per one multiple upload!";
                            popup();
                        }

                    }
                    else
                    {
                        error += "Please select an option...";
                        popup();
                    }
                }
                else
                {
                    error += "Please enter ";
                    if (type == "1")
                    {
                        error += "Password to continue";
                    }
                    else
                    {
                        error += "location co-ordinates to continue";
                    }
                    popup();
                }
            }
            else
            {
                error += "Oops! You are not connected to the internet! \nPlease check your internet connection and try again...";
                popup();
            }

        }

        private async void create_table_mutiple(multiple_storage item)
        {
            try
            {
                string key = connection.get_key();
                string tableName = "multiple";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                CloudTable table = tableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();

                TableOperation insertOperation = TableOperation.InsertOrReplace(item);

                await table.ExecuteAsync(insertOperation);
            }
            catch
            {
                error += "Error in Entering table...\nPlease Try again.";
                popup();
            }
        }
        private async Task<string> generate_upload_multiple(IReadOnlyList<StorageFile> files)
        {
            string key = connection.get_key();
            string containerName = "public";
            string blob = "HLIN";
            int count = 1;
            foreach (StorageFile file in files)
            {
                blob = "HLIN";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                CloudBlockBlob counterBlob = container.GetBlockBlobReference("counter");

                string content = await counterBlob.DownloadTextAsync();


                int number = Int32.Parse(content) + 1;
                string rowkey = (number.ToString());
                blob += type;
                blob += rowkey;
                loading_text.Text = "Uploading..." + "\nFile:" + count.ToString() + "/" + files.Count.ToString();


                await create_blob(key, containerName, blob, file);

                storage item = new storage("Public", rowkey, password.Text.ToString(), latitude.Text.ToString(), longitude.Text.ToString(), containerName, file.FileType.ToString(), blob, DateTime.Now, slider.Value, ssid);
                create_table(key, "users", item);
                await update_counter(key, containerName, rowkey);
                count++;
            }
            loading_text.Text = "Finalising...";
            multiple_storage temp = new multiple_storage("Public", "M" + blob, userkey, files.Count.ToString(), password.Text.ToString(), latitude.Text.ToString(), longitude.Text.ToString(), containerName, blob, DateTime.Now, slider.Value, ssid);
            create_table_mutiple(temp);
            loading_text.Text = "Upload done";
            return "M" + blob;
        }

        private void network_lock_Checked(object sender, RoutedEventArgs e)
        {
            password.Visibility = Visibility.Collapsed;
            latitude.Visibility = Visibility.Collapsed;
            longitude.Visibility = Visibility.Collapsed;
            getLocationCheck.Visibility = Visibility.Collapsed;
            slider.Visibility = Visibility.Collapsed;
            slider_text.Visibility = Visibility.Collapsed;
            SSID_text.Visibility = Visibility.Visible;
            ssid = "";
            var icp = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (icp != null)
            {
                if (icp.WlanConnectionProfileDetails != null)
                {
                    ssid = icp.WlanConnectionProfileDetails.GetConnectedSsid();
                    SSID_text.Text = "Your Current connected wifi network:\n\t" + ssid;
                    type = "3";
                }
                else
                {
                    error = "Please Connect to some wifi network to enable the lock";
                    popup();
                }
            }
        }

        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }
    }
}