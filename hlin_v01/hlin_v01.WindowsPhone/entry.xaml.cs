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
using Windows.Devices.Geolocation;
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
using Windows.Phone.UI.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace hlin_v01
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class entry : Page
    {
        public entry()
        {
            this.InitializeComponent();
        }
        string error = "";
        
        string title = "Error:";
        config connection = new config();
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter == null || string.IsNullOrWhiteSpace(e.Parameter.ToString())))
            {
                var parameter = e.Parameter as string;
                userid.Text += parameter;
            }
            password.Visibility = Visibility.Collapsed;
            next.Visibility = Visibility.Collapsed;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
            e.Handled = true;
        }
        private async void next_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    show_progress();
                    invisible();
                    await validate();
                    hide_progress();
                    result.Visibility = Visibility.Visible;
                }
                else
                {
                    error += "Oops! You are not connected to the internet! \nPlease check your internet connection and try again...";

                    popup();
                    visible();
                    next.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                error += "Enter the name of the file to save to...";
                popup();
            }


        }


        private async Task validate()
        {
            string key = connection.get_key();
            string containerName = "public";
            loading_text.Text = "Verifying...";
            if (userid.Text.ToString() != "")
            {
                var arr = (userid.Text.ToString()).ToCharArray();
                if (arr[0] == 'M')
                {
                    switch (arr[5])
                    {
                        case '0':
                            await download_multiple(key, containerName);
                            break;
                        case '1':
                            if (password.Text != "")
                            {
                                await check_password(key, containerName, true);
                            }
                            else
                            {
                                error += "Please enter the password... ";
                                popup();
                                visible();
                            }
                            break;
                        case '2':
                            await check_location(key, true);
                            break;
                        case '3':
                            await network_lock(key, true);
                            break;
                    }
                    loading_text.Text = "Download Complete...";
                }
                else {
                    switch (arr[4])
                    {
                        case '0':
                            storage obj = new storage();
                            obj = await get_file_name(key, false, "");

                            await download(key, containerName, obj.blobName + obj.extension, "");
                            break;
                        case '1':
                            if (password.Text != "")
                            {
                                await check_password(key, containerName, false);
                            }
                            else
                            {
                                error += "Please enter the password... ";
                                popup();
                                visible();
                            }
                            break;
                        case '2':
                            await check_location(key, false);
                            break;
                        case '3':
                            await network_lock(key, false);
                            break;
                    }
                    loading_text.Text = "Download Complete...";
                }
            }
            else
            {
                error += "Please enter the Access Key and try again...";
                popup();
                visible();
            }
        }



        private async Task download(string key, string containerName, string filename, string blob)
        {

            if (filename != "")
            {
                if (blob == "")
                {
                    loading_text.Text = "Connecting to the server...";
                }

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                string temp = "";
                if (blob == "")
                {
                    loading_text.Text = "Downloading...";
                    temp = userid.Text.ToString();
                }
                else
                {
                    temp = blob;
                }


                // Retrieve reference to a blob named "photo1.jpg".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(temp);

                
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                try
                {
                    await blockBlob.DownloadToFileAsync(file);
                    if (blob == "")
                    {
                        result.Text = "File has been downloaded...";
                    }

                }
                catch
                {
                    error += "File is not present in the server";
                    popup();
                    visible();
                }
                //\nFile path:" + file.Path.ToString();
                //result.Text += "\nFile name:" + file.Name.ToString();
            }
            else
            {
                error += "File is not present in the server";
                popup();
                visible();
            }

        }

        private async Task download_multiple(string key, string containerName)
        {

            multiple_storage objs = new multiple_storage();
            objs = await get_files_name(key);
            int count;
            Int32.TryParse(objs.count, out count);
            string rem, counter;
            int num;
            for (int i = 0; i < count; i++)
            {
                int index = 0;
                counter = (index < 0) ? objs.blobName.ToString() : objs.blobName.ToString().Remove(index, 5);
                index = 5;
                rem = (index < 0) ? objs.blobName.ToString() : objs.blobName.ToString().Remove(index, 4);
                Int32.TryParse(counter, out num);
                rem += (num - i).ToString();
                storage obj = await get_file_name(key, true, rem);
                loading_text.Text = "Downloading ...\nFile :" + (i + 1).ToString() + "/" + count.ToString(); ;
                await download(key, containerName, obj.blobName + obj.extension, obj.blobName);


            }
            result.Text = "File has been downloaded...";

        }

        //get_file_name
        private async Task<storage> get_file_name(string key, bool mul, string id)
        {
            /* string removeString = "HLIN";
             string sourceString = userid.Text.ToString();
             string blob = sourceString.Remove(sourceString.IndexOf(removeString), removeString.Length); */
            int index = 0;
            string counter = "";
            if (mul)
            {
                counter = (index < 0) ? id.ToString() : id.Remove(index, 5);

            }
            else
            {
                counter = (index < 0) ? userid.Text.ToString() : userid.Text.ToString().Remove(index, 5);
            }



            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("user");

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<storage>("Public", counter);

            // Execute the retrieve operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);


            return ((storage)retrievedResult.Result);

        }
        private async Task<multiple_storage> get_files_name(string key)
        {
            /* string removeString = "HLIN";
             string sourceString = userid.Text.ToString();
             string blob = sourceString.Remove(sourceString.IndexOf(removeString), removeString.Length); */
            string blob = userid.Text.ToString();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("multiple");

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<multiple_storage>("Public", blob);

            // Execute the retrieve operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);


            return ((multiple_storage)retrievedResult.Result);

        }
        //filename function ends



        //checking function

        //password_lock
        private async Task check_password(string key, string containerName, bool mul)
        {


            string pass, filename;
            if (mul)
            {
                multiple_storage obj = new multiple_storage();
                obj = await get_files_name(key);
                filename = obj.blobName + obj.extension;
                pass = obj.password;
            }
            else
            {
                storage obj = new storage();
                obj = await get_file_name(key, mul, "");
                filename = obj.blobName + obj.extension;
                pass = obj.password;
            }


            if (pass == password.Text.ToString())
            {
                if (mul)
                {
                    await download_multiple(key, containerName);
                }
                else
                {
                    await download(key, containerName, filename, "");
                }

            }
            else
            {
                error += "Access Denied ... ";
                popup();
                visible();
            }
        }


        //geo_lock   
        private async Task check_location(string key, bool mul)
        {
            string latitude, longitude;
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracy = Windows.Devices.Geolocation.PositionAccuracy.High;
            geolocator.DesiredAccuracyInMeters = 50;
            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1));
                latitude = geoposition.Coordinate.Point.Position.Latitude.ToString("0.0000000000");
                longitude = geoposition.Coordinate.Point.Position.Longitude.ToString("0.0000000000");


                string olat, olon, filename, containerName;
                double perimeter = 50;
                if (mul)
                {
                    multiple_storage obj = new multiple_storage();
                    obj = await get_files_name(key);
                    filename = obj.blobName + obj.extension;
                    olat = obj.latitude;
                    olon = obj.longitude;
                    containerName = obj.containerName;
                    perimeter = obj.perimeter;
                }
                else
                {
                    storage obj = new storage();
                    obj = await get_file_name(key, mul, "");
                    filename = obj.blobName + obj.extension;
                    olat = obj.latitude;
                    olon = obj.longitude;
                    perimeter = obj.perimeter;
                    containerName = obj.containerName;
                }

                double R = 6371; // km
                double lat1, lat2, lon1, lon2;
                Double.TryParse(latitude, out lat1);
                Double.TryParse(olat, out lat2);
                Double.TryParse(longitude, out lon1);
                Double.TryParse(olon, out lon2);
                double dLat = lat2 - lat1;
                double dLon = lon2 - lon1;


                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                double d = R * c;
                if (d < perimeter / 10)
                {
                    if (mul)
                    {
                        await download_multiple(key, containerName);
                    }
                    else
                    {
                        await download(key, containerName, filename, "");
                    }

                }
                else
                {
                    error += "Access Denied ...";
                    popup();
                    visible();
                }


            }
            catch
            {
                error += "Error in fetching latitude...\n Try again...";
                popup();
                visible();
            }



        }


        private async Task network_lock(string key, bool mul)
        {
            string ssid = "";
            var icp = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (icp != null)
            {
                if (icp.WlanConnectionProfileDetails != null)
                {
                    ssid = icp.WlanConnectionProfileDetails.GetConnectedSsid();
                }
                else
                {
                    error = "Please connect to the required wifi network...";
                    popup();
                    visible();
                }
            }

            if (mul)
            {
                multiple_storage obj = new multiple_storage();
                obj = await get_files_name(key);
                if (ssid == obj.ssid)
                {
                    await download_multiple(key, obj.containerName);
                }
                else
                {
                    error = "Access Denied...";
                    popup();
                    visible();
                }

            }
            else
            {
                storage obj = new storage();
                obj = await get_file_name(key, mul, "");
                if (ssid == obj.ssid)
                {
                    await download(key, obj.containerName, obj.blobName + obj.extension, "");
                }
                else
                {
                    error = "Access Denied...";
                    popup();
                    visible();
                }
            }
        }

        //checking function ends



        // pop up function starts
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
            error = "";
            title = "Error:";

        }
        //popup function ends
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void verify_Click(object sender, RoutedEventArgs e)
        {
            if (userid.Text != "")
            {
                try
                {
                    string temp = userid.Text.ToString();

                    if ((temp.Length < 10) && (temp.Contains("HLIN")))
                    {
                        char[] id = temp.ToCharArray();
                        next.Visibility = Visibility.Visible;
                        verify.Visibility = Visibility.Collapsed;
                        if (id[4] == '1')
                        {
                            password.Visibility = Visibility.Visible;
                        }
                    }
                    else if ((temp.Length < 11) && (temp.Contains("MHLIN")))
                    {
                        char[] id = temp.ToCharArray();
                        next.Visibility = Visibility.Visible;
                        verify.Visibility = Visibility.Collapsed;
                        if (id[5] == '1')
                        {
                            password.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        error += "Enter correct Access Key...";
                        popup();
                    }

                }
                catch
                {
                    error += "Enter correct Access Key...";
                    popup();
                }
            }
            else
            {
                error += "Please enter Access Key...";
                popup();
            }
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
            userid.Visibility = Visibility.Collapsed;
            password.Visibility = Visibility.Collapsed;
            verify.Visibility = Visibility.Collapsed;
            next.Visibility = Visibility.Collapsed;

        }
        private void visible()
        {
            userid.Visibility = Visibility.Visible;
            verify.Visibility = Visibility.Visible;

        }
        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(feedback));
        }
    }
}
