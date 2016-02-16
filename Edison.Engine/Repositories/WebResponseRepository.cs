/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System.IO;
using Edison.Engine.Repositories.Interfaces;
using System.Net;
using Edison.Injector;

namespace Edison.Engine.Repositories
{
    [InjectionInterface(typeof(IWebResponseRepository))]
    public class WebResponseRepository : IWebResponseRepository
    {
        
        private WebResponse Response;

        public WebResponseRepository(WebResponse response)
        {
            Response = response;
        }

        public void Dispose()
        {
            if (Response != default(WebResponse))
            {
                Response.Dispose();
            }
        }

        public Stream GetResponseStream()
        {
            return Response.GetResponseStream();
        }

    }
}
