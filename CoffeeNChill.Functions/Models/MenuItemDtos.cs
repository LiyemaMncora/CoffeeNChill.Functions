using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeNChill.Functions.Models
{
    public class MenuItemRequest
    {
        public string Category { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class MenuItemResponse
    {
        public string Category { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
    }

    public static class MenuItemExtensions
    {
        public static MenuItemResponse ToResponse(this MenuItem entItem)
        {
            return new MenuItemResponse
            {
                Category = entItem.PartitionKey,
                Id = entItem.RowKey,
                Name = entItem.Name,
                Description = entItem.Description,
                Price = entItem.Price,
                IsAvailable = entItem.IsAvailable
            };
        }
    }
}