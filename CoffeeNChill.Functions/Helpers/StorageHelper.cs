using System;
using System.Collections.Generic;
using System.Text;
using Azure.Data.Tables;
using Azure.Storage.Blobs;

namespace CoffeeNChill.Functions.Helpers
{
    public static class StorageHelper
    {
        private static readonly string strConnString =
            Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? "UseDevelopmentStorage=true";

        public static TableClient GetMenuTable()
        {
            TableServiceClient tblService = new TableServiceClient(strConnString);
            TableClient tblMenu = tblService.GetTableClient("MenuItems");
            tblMenu.CreateIfNotExists();
            return tblMenu;
        }

        public static BlobContainerClient GetStaffDocsContainer()
        {
            BlobServiceClient blbService = new BlobServiceClient(strConnString);
            BlobContainerClient blbContainer = blbService.GetBlobContainerClient("staff-docs");
            blbContainer.CreateIfNotExists();
            return blbContainer;
        }
    }
}