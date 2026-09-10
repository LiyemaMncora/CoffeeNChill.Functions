using System;
using System.Collections.Generic;
using System.Text;
using Azure;
using Azure.Data.Tables;

namespace CoffeeNChill.Functions.Models
{
    // PartitionKey, RowKey, Timestamp and ETag are required exact names —
    // the Azure SDK maps to them directly, so they skip our naming style.
    public class MenuItem : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty; // Category
        public string RowKey { get; set; } = string.Empty;       // SKU / Item ID
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
