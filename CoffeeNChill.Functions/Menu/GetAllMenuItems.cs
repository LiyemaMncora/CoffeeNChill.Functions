using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Models;
using CoffeeNChill.Functions.Helpers;

namespace CoffeeNChill.Functions.Menu
{
    public class GetAllMenuItems
    {
        [Function("GetAllMenuItems")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")] HttpRequestData req)
        {
            var tblMenu = StorageHelper.GetMenuTable();
            List<MenuItem> lstItems = new List<MenuItem>();

            await foreach (MenuItem entItem in tblMenu.QueryAsync<MenuItem>())
            {
                lstItems.Add(entItem);
            }

            HttpResponseData resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteAsJsonAsync(lstItems.Select(x => x.ToResponse()));
            return resp;
        }
    }
}