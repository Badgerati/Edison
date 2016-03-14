/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;
using Edison.Engine.Core.Exceptions;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System;
using System.IO;

namespace Edison.Engine.Validators
{
    public class TestResultUrlValidator : IValidator
    {

        #region Repositories

        private static IWebRequestRepository WebRequestRepository
        {
            get { return DIContainer.Instance.Get<IWebRequestRepository>(); }
        }

        #endregion

        #region Validate

        public void Validate(EdisonContext context)
        {
            if (string.IsNullOrWhiteSpace(context.TestResultURL))
            {
                return;
            }

            try
            {
                var request = WebRequestRepository.Create(context.TestResultURL);
                request.Timeout = 30;

                using (var response = request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ValidationException("Connection to provided TestRunURL ('{0}') failed:{1}{2}", context.TestResultURL, Environment.NewLine, ex.Message);
            }
        }

        #endregion

    }
}
