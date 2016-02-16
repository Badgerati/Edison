/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System.IO;

namespace Edison.Engine.Repositories.Interfaces
{
    public interface IWebRequestRepository
    {

        int Timeout { get; set; }
        string Method { get; set; }
        string ContentType { get; set; }
        long ContentLength { get; set; }

        IWebRequestRepository Create(string url);
        IWebResponseRepository GetResponse();
        Stream GetRequestStream();

    }
}
