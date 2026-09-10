using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using CoffeeNChill.Functions.Models;
using CoffeeNChill.Functions.Helpers;

namespace CoffeeNChill.Functions.Menu
{
    public class CreateMenuItem
    {
        [Function("CreateMenuItem")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")] HttpRequestData req)
        {
            string strBody = await new StreamReader(req.Body).ReadToEndAsync();
            MenuItemRequest? objInput = JsonSerializer.Deserialize<MenuItemRequest>(strBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (objInput == null || string.IsNullOrWhiteSpace(objInput.Category) || string.IsNullOrWhiteSpace(objInput.Id))
            {
                HttpResponseData respBad = req.CreateResponse(HttpStatusCode.BadRequest);
                await respBad.WriteStringAsync("\"category\" and \"id\" are required.");
                return respBad;
            }

            MenuItem entItem = new MenuItem
            {
                PartitionKey = objInput.Category,
                RowKey = objInput.Id,
                Name = objInput.Name ?? string.Empty,
                Description = objInput.Description ?? string.Empty,
                Price = objInput.Price,
                IsAvailable = objInput.IsAvailable
            };

            var tblMenu = StorageHelper.GetMenuTable();
            await tblMenu.AddEntityAsync(entItem);

            HttpResponseData resp = req.CreateResponse(HttpStatusCode.Created);
            await resp.WriteAsJsonAsync(entItem.ToResponse());
            return resp;
        }
    }
}