using Library.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Extensions
{
    public static class PaginationHelpers
    {
        public static void CreatePaginationLinks<T>(this PaginatedList<T> list, IUrlHelper urlHelper, string routeName, object routeValues)
        {
            if (list.PageIndex > 1)
            {
                list.Links.Add(CreateLink(urlHelper, routeName, routeValues, list.PageIndex - 1, "previousPage", "GET"));
            }

            if (list.HasNextPage)
            {
                list.Links.Add(CreateLink(urlHelper, routeName, routeValues, list.PageIndex + 1, "nextPage", "GET"));
            }

            list.Links.Add(CreateLink(urlHelper, routeName, routeValues, 1, "firstPage", "GET"));
            list.Links.Add(CreateLink(urlHelper, routeName, routeValues, list.TotalPages, "lastPage", "GET"));
        }

        private static LinkDto CreateLink(IUrlHelper urlHelper, string routeName, object routeValues, int pageNumber, string rel, string method)
        {
            var values = new RouteValueDictionary(routeValues) { ["PageNumber"] = pageNumber };
            var href = urlHelper.Link(routeName, values);
            return new LinkDto(href, rel, method);
        }
    }
}