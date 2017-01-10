using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage.Table;

namespace hlin_v01
{
    class storage : TableEntity
    {
     
        public string password { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string containerName { get; set; }
        public string extension { get; set; }
        public string blobName { get; set; }
        public DateTime date { get; set; }
        public double perimeter { get; set; }
        public string ssid { get; set; }

        public storage() { }
        public storage(string partitionKey ,string rowKey ,string Password,string Latitude,string Longitude,string ContainerName,string Extension,string BlobName, DateTime Date,double Perimeter,string SSID)
        {
          this.PartitionKey = partitionKey;
          this.RowKey = rowKey;
          this.password = Password;
          this.latitude = Latitude;
          this.longitude = Longitude;
          this.extension = Extension;
          this.blobName = BlobName;
          this.containerName = ContainerName;
          this.date = Date;
          this.perimeter = Perimeter;
            this.ssid = SSID;

        }
    }
   class multiple_storage : TableEntity
    {
        public string end { get; set; }
        public string count { get; set; }
        public string password { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string containerName { get; set; }
        public string extension { get; set; }
        public string blobName { get; set; }
        public DateTime date { get; set; }
        public double perimeter { get; set; }
        public string ssid { get; set; }

        public multiple_storage() { }

        public multiple_storage(string partitionKey, string rowKey,string End ,string Count,string Password, string Latitude, string Longitude, string ContainerName, string BlobName, DateTime Date,double Perimeter, string SSID)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
            this.end = End;
            this.count = Count;
            this.password = Password;
            this.latitude = Latitude;
            this.longitude = Longitude;
            this.blobName = BlobName;
            this.containerName = ContainerName;
            this.date = Date;
            this.perimeter = Perimeter;
            this.ssid = SSID;
        }
    }
    



}

   

