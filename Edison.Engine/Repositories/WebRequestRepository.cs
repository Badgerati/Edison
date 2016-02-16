/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System.IO;
using System.Net;

namespace Edison.Engine.Repositories
{
    [InjectionInterface(typeof(IWebRequestRepository))]
    public class WebRequestRepository : IWebRequestRepository
    {

        #region Instance Variables

        private WebRequest Request;

        #endregion

        #region Properties

        public int Timeout
        {
            get { return Request.Timeout; }
            set { Request.Timeout = value; }
        }

        public string Method
        {
            get { return Request.Method; }
            set { Request.Method = value; }
        }

        public string ContentType
        {
            get { return Request.ContentType; }
            set { Request.ContentType = value; }
        }

        public long ContentLength
        {
            get { return Request.ContentLength; }
            set { Request.ContentLength = value; }
        }

        #endregion

        #region Public Helpers

        public IWebRequestRepository Create(string url)
        {
            Request = WebRequest.Create(url);
            return this;
        }

        public IWebResponseRepository GetResponse()
        {
            return new WebResponseRepository(Request.GetResponse());
        }

        public Stream GetRequestStream()
        {
            return Request.GetRequestStream();
        }

        #endregion

    }
}
