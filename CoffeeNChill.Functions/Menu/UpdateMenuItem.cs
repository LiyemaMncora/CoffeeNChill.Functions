using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Models;
using CoffeeNChill.Functions.Helpers;

namespace CoffeeNChill.Functions.Menu
{
    public class UpdateMenuItem
    {
        [Function("UpdateMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "menu/{category}/{id}")] HttpRequestData req,
            string category, string id)
        {
            var tblMenu = StorageHelper.GetMenuTable();
            MenuItem entItem;

            try
            {
                Response<MenuItem> respExisting = await tblMenu.GetEntityAsync<MenuItem>(category, id);
                entItem = respExisting.Value;
            }
            catch (RequestFailedException)
            {
                HttpResponseData respNotFound = req.CreateResponse(HttpStatusCode.NotFound);
                await respNotFound.WriteStringAsync($"No menu item \"{id}\" found in category \"{category}\".");
                return respNotFound;
            }

            string strBody = await new StreamReader(req.Body).ReadToEndAsync();
            MenuItemRequest? objUpdate = JsonSerializer.Deserialize<MenuItemRequest>(strBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (objUpdate != null)
            {
                if (!string.IsNullOrWhiteSpace(objUpdate.Name)) entItem.Name = objUpdate.Name;
                if (!string.IsNullOrWhiteSpace(objUpdate.Description)) entItem.Description = objUpdate.Description;
                entItem.Price = objUpdate.Price;
                entItem.IsAvailable = objUpdate.IsAvailable;
            }

            await tblMenu.UpdateEntityAsync(entItem, entItem.ETag, TableUpdateMode.Replace);

            HttpResponseData resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteAsJsonAsync(entItem.ToResponse());
            return resp;
        }
    }
}