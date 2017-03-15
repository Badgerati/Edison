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
        /// Gets or sets the test run's informative name.
        /// </summary>
        /// <value>
        /// The test run's informative name.
        /// </value>
        public string TestRunName { get; set; }

        /// <summary>
        /// Gets or sets the test run's project name.
        /// </summary>
        /// <value>
        /// The test run's project name.
        /// </value>
        public string TestRunProject { get; set; }

        /// <summary>
        /// Gets or sets the name of the environment that the test run occurred.
        /// </summary>
        /// <value>
        /// The name of the environment the rest run occurred.
        /// </value>
        public string TestRunEnvironment { get; set; }

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
