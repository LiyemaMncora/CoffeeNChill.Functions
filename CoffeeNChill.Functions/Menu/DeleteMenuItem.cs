using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Helpers;

namespace CoffeeNChill.Functions.Menu
{
    public class DeleteMenuItem
    {
        [Function("DeleteMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "menu/{category}/{id}")] HttpRequestData req,
            string category, string id)
        {
            var tblMenu = StorageHelper.GetMenuTable();

            try
            {
                await tblMenu.DeleteEntityAsync(category, id);
            }
            catch (RequestFailedException)
            {
                HttpResponseData respNotFound = req.CreateResponse(HttpStatusCode.NotFound);
                await respNotFound.WriteStringAsync($"No menu item \"{id}\" found in category \"{category}\".");
                return respNotFound;
            }

            HttpResponseData resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteStringAsync($"Deleted \"{id}\" from \"{category}\".");
            return resp;
        }
    }
}