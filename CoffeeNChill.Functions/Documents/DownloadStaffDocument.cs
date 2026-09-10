using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Helpers;

namespace CoffeeNChill.Functions.Documents
{
    public class DownloadStaffDocument
    {
        [Function("DownloadStaffDocument")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents/download/{fileName}")] HttpRequestData req,
            string fileName)
        {
            var blbContainer = StorageHelper.GetStaffDocsContainer();
            var blbFile = blbContainer.GetBlobClient(fileName);

            bool blnExists = await blbFile.ExistsAsync();
            if (!blnExists)
            {
                HttpResponseData respNotFound = req.CreateResponse(HttpStatusCode.NotFound);
                await respNotFound.WriteStringAsync($"File \"{fileName}\" was not found.");
                return respNotFound;
            }

            var respDownload = await blbFile.DownloadStreamingAsync();

            HttpResponseData resp = req.CreateResponse(HttpStatusCode.OK);
            resp.Headers.Add("Content-Type", respDownload.Value.Details.ContentType ?? "application/octet-stream");
            resp.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
            await respDownload.Value.Content.CopyToAsync(resp.Body);
            return resp;
        }
    }
}