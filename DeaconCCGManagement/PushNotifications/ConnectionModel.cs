using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace DeaconCCGManagement.PushNotifications
{
    public class ConnectionModel : TableEntity
    {

        public DateTime ConnectionTimeStamp { get; set; }

        public ConnectionModel(string userId, string connectionId, DateTime timeStamp)
        {
            //
            // PartitionKey and RowKey in Windows Azure Table Storage:
            // https://dzone.com/articles/partitionkey-and-rowkey
            //
            this.PartitionKey = userId;
            this.RowKey = connectionId;
            ConnectionTimeStamp = timeStamp;
        }

        public ConnectionModel() { }
    }
}