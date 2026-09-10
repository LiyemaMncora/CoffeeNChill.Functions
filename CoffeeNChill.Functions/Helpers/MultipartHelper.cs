using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Net.Http.Headers;

namespace CoffeeNChill.Functions.Helpers
{
    public static class MultipartHelper
    {
        public static string GetBoundary(string strContentType)
        {
            MediaTypeHeaderValue mthHeader = MediaTypeHeaderValue.Parse(strContentType);
            string strBoundary = HeaderUtilities.RemoveQuotes(mthHeader.Boundary).Value ?? string.Empty;

            if (string.IsNullOrEmpty(strBoundary))
            {
                throw new InvalidDataException("No boundary found in the Content-Type header.");
            }

            return strBoundary;
        }
    }
}