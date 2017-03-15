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

        private WebRequest _request;

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
            get { return _request.Timeout; }
            set { _request.Timeout = value; }
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public string Method
        {
            get { return _request.Method; }
            set { _request.Method = value; }
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType
        {
            get { return _request.ContentType; }
            set { _request.ContentType = value; }
        }

        /// <summary>
        /// Gets or sets the length of the content.
        /// </summary>
        /// <value>
        /// The length of the content.
        /// </value>
        public long ContentLength
        {
            get { return _request.ContentLength; }
            set { _request.ContentLength = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Basic public constructor.
        /// </summary>
        public WebRequestRepository() { }

        /// <summary>
        /// Private constructor to create an instance of a repositorised HTTP request.
        /// </summary>
        /// <param name="url">The URL to create the request against.</param>
        private WebRequestRepository(string url)
        {
            _request = WebRequest.Create(url);
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
            return new WebRequestRepository(url);
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <returns></returns>
        public IWebResponseRepository GetResponse()
        {
            return new WebResponseRepository(_request.GetResponse());
        }

        /// <summary>
        /// Gets the request stream.
        /// </summary>
        /// <returns></returns>
        public Stream GetRequestStream()
        {
            return _request.GetRequestStream();
        }

        #endregion

    }
}
