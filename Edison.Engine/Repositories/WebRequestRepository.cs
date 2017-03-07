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

        /// <summary>
        /// Gets or sets the timeout, in milliseconds.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public int Timeout
        {
            get { return Request.Timeout; }
            set { Request.Timeout = value; }
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public string Method
        {
            get { return Request.Method; }
            set { Request.Method = value; }
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType
        {
            get { return Request.ContentType; }
            set { Request.ContentType = value; }
        }

        /// <summary>
        /// Gets or sets the length of the content.
        /// </summary>
        /// <value>
        /// The length of the content.
        /// </value>
        public long ContentLength
        {
            get { return Request.ContentLength; }
            set { Request.ContentLength = value; }
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Create a web request using the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>A web request.</returns>
        public IWebRequestRepository Create(string url)
        {
            Request = WebRequest.Create(url);
            return this;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <returns></returns>
        public IWebResponseRepository GetResponse()
        {
            return new WebResponseRepository(Request.GetResponse());
        }

        /// <summary>
        /// Gets the request stream.
        /// </summary>
        /// <returns></returns>
        public Stream GetRequestStream()
        {
            return Request.GetRequestStream();
        }

        #endregion

    }
}
