using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Helpers;

namespace CoffeeNChill.Functions.Documents
{
    public class ListStaffDocuments
    {
        [Function("ListStaffDocuments")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents")] HttpRequestData req)
        {
            var blbContainer = StorageHelper.GetStaffDocsContainer();
            List<object> lstFiles = new List<object>();

            await foreach (var blbItem in blbContainer.GetBlobsAsync())
            {
                lstFiles.Add(new
                {
                    fileName = blbItem.Name,
                    sizeBytes = blbItem.Properties.ContentLength,
                    lastModified = blbItem.Properties.LastModified
                });
            }

            HttpResponseData resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteAsJsonAsync(lstFiles);
            return resp;
        }
    }
}