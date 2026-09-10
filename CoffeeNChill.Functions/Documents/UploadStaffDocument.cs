using CoffeeNChill.Functions.Helpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CoffeeNChill.Functions.Documents
{
    public class UploadStaffDocument
    {
        [Function("UploadStaffDocument")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "documents/upload")] HttpRequestData req)
        {
            string? strContentType = req.Headers.TryGetValues("Content-Type", out var colValues)
                ? colValues.FirstOrDefault()
                : null;

            if (string.IsNullOrEmpty(strContentType) || !strContentType.Contains("multipart/"))
            {
                HttpResponseData respBad = req.CreateResponse(HttpStatusCode.BadRequest);
                await respBad.WriteStringAsync("Request must be sent as multipart/form-data.");
                return respBad;
            }

            string strBoundary = MultipartHelper.GetBoundary(strContentType);
            MultipartReader mpReader = new MultipartReader(strBoundary, req.Body);

            string strFileName = string.Empty;
            MemoryStream msFile = new MemoryStream();

            MultipartSection? mpSection = await mpReader.ReadNextSectionAsync();
            while (mpSection != null)
            {
                var dchDisposition = mpSection.GetContentDispositionHeader();

                if (dchDisposition != null && dchDisposition.IsFileDisposition())
                {
                    strFileName = dchDisposition.FileName.Value ?? "upload.dat";
                    await mpSection.Body.CopyToAsync(msFile);
                }

                mpSection = await mpReader.ReadNextSectionAsync();
            }

            if (string.IsNullOrEmpty(strFileName) || msFile.Length == 0)
            {
                HttpResponseData respBad = req.CreateResponse(HttpStatusCode.BadRequest);
                await respBad.WriteStringAsync("No file was found in the request body. Use form-data with a file field.");
                return respBad;
            }

            msFile.Position = 0;
            var blbContainer = StorageHelper.GetStaffDocsContainer();
            var blbFile = blbContainer.GetBlobClient(strFileName);
            await blbFile.UploadAsync(msFile, overwrite: true);

            HttpResponseData resp = req.CreateResponse(HttpStatusCode.Created);
            await resp.WriteStringAsync($"Uploaded \"{strFileName}\" to staff-docs.");
            return resp;
        }
    }
}