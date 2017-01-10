using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace hlin_v01
{
    class temp_obj : TableEntity
    {
        
        public string remember { get; set; }

        public temp_obj(string Remember, string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
            this.remember = Remember;
        }
    }

}
