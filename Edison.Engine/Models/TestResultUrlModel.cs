/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Engine.Models
{
    public class TestResultUrlModel
    {
        /// <summary>
        /// Gets or sets the test run identifier.
        /// </summary>
        /// <value>
        /// The test run identifier.
        /// </value>
        public string TestRunId { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the action type for the current URL callout.
        /// </summary>
        /// <value>
        /// The action type.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public TestResultUrlActionType Action { get; set; }

        /// <summary>
        /// Gets or sets the test results.
        /// </summary>
        /// <value>
        /// The test results.
        /// </value>
        public TestResultModel[] TestResults { get; set; }
    }
}
