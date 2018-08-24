using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.Services;

namespace DeaconCCGManagement.Helpers
{
    public static class ViewHelper
    {
        static Random randomizer = new Random();

        public static string NoPhotoImgPath = "/Content/Images/No Photo.png";

        public static string GetRandomString()
        {
            char[] chars = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p' };

            var sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(chars[randomizer.Next(16)]);
            }

            return sb.ToString();
        }

        public static string GetUrlWithPerPageQuery(int itemsPerPage, string absPath,
            NameValueCollection queryStrings)
        {
            // Example url: 
            // mydomain.com/Members/Index?id=3&getAll=true&itemsPerPage=25
            return UrlWithPerPageQuery(itemsPerPage, absPath, queryStrings, nameof(itemsPerPage));
        }

        public static string GetUrlWithPerPageQuery(string absPath,
            NameValueCollection queryStrings, string queryName)
        {
            // Example url: 
            // mydomain.com/Members/Index?id=3&getAll=true&listAll=true
            return UrlWithPerPageQuery(null, absPath, queryStrings, queryName);
        }

        private static string UrlWithPerPageQuery(int? itemsPerPage, string absPath,
            NameValueCollection queryStrings, string queryName)
        {
            // only one query string in collection and that one is the given query name
            // Example: the one query is 'itemsPerPage'
            bool oneKeyIsQueryName =
                queryStrings.Count == 1
                && queryStrings.GetKey(0).Equals(queryName);

            string prefix = oneKeyIsQueryName || !queryStrings.HasKeys() ? "" : "&";
            var sb = new StringBuilder();
            sb.Append($"{absPath}?");

            if (queryStrings.HasKeys() && !oneKeyIsQueryName)
            {
                ConcatQueryStrings(queryStrings, sb, queryName);
            }
            sb.Append(itemsPerPage != null ? $"{prefix}{queryName}={itemsPerPage}"
                : $"{prefix}{queryName}={true}");

            return sb.ToString();
        }

        private static void ConcatQueryStrings(NameValueCollection queryStrings,
            StringBuilder sb, string queryName)
        {
            string prefix = "";
            int index = 0;
            foreach (var key in queryStrings.AllKeys)
            {
                string keyName = queryStrings.GetKey(index);

                // Eg.: 'itemsPerPage' and/or 'listAll' could already exists in query string
                // Skip if no value for key
                if (!keyName.Equals(queryName)
                    && !keyName.Equals("itemsPerPage")
                    && !keyName.Equals("listAll")
                    && !string.IsNullOrEmpty(queryStrings[key]))
                {
                    sb.Append($"{prefix}{keyName}={queryStrings[key]}");
                    prefix = "&";
                }
                index++;
            }
        }

        public static bool DoesUserHavePhoto(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return false;
            using (var unitOfwork = new UnitOfWork())
            {
                var service = new MemberPhotoService(unitOfwork);
                return service.DoesMemberHavePhoto(null, userId);
            }
        }
    }
}