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
            // only validate if we have a URL supplied
            if (string.IsNullOrWhiteSpace(context.TestResultURL))
            {
                return;
            }

            // first, was a TestRunId supplied?
            if (string.IsNullOrWhiteSpace(context.TestRunId))
            {
                throw new ValidationException("A TestRunId is required when sending results to a URL");
            }

            // check the TestRunName, if none then set it to the TestRunId
            if (string.IsNullOrWhiteSpace(context.TestRunName))
            {
                context.TestRunName = context.TestRunId;
            }

            // now ensure the URL is contactable
            try
            {
                var request = WebRequestRepository.Create(context.TestResultURL);
                request.Method = "POST";
                request.Timeout = 30000;

                using (var response = request.GetResponse()) { }
            }
            catch (Exception ex)
            {
                throw new ValidationException("Connection to provided TestRunURL ('{0}') failed:{1}{2}", context.TestResultURL, Environment.NewLine, ex.Message);
            }
        }

        #endregion

    }
}
