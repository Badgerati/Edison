/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Net;

namespace Edison.Framework
{
    public static class BrowserHelper
    {

        public static bool ValidateUrl(string url)
        {            
            try
            {
                var urlNoParams = (new Uri(url)).GetLeftPart(UriPartial.Path);

                var request = WebRequest.Create(urlNoParams);
                request.Method = "GET";

                using (var response = request.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException wex)
            {
                var statusCode = HttpStatusCode.OK;
                var statusDesc = string.Empty;

                if (wex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ((HttpWebResponse)wex.Response);
                    statusCode = response.StatusCode;
                    statusDesc = response.StatusDescription;
                }
                else
                {
                    statusCode = HttpStatusCode.Unauthorized;
                    statusDesc = string.Format("No response in failure object, Status: {0}", wex.Status);
                }

                throw new ArgumentException(string.Format("The URL '{0}'. does not exist:\nStatus Code: {1}\nStatus Desc: {2}", url, statusCode, statusDesc));
            }

        }

    }
}
