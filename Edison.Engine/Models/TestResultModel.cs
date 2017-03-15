/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Edison.Engine.Models
{
    public class TestResultModel
    {

        #region Properties

        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        /// <value>
        /// The name of the test.
        /// </value>
        public string TestName { get; set; }

        /// <summary>
        /// Gets or sets the name space.
        /// </summary>
        /// <value>
        /// The name space.
        /// </value>
        public string NameSpace { get; set; }

        /// <summary>
        /// Gets or sets the state of the test.
        /// </summary>
        /// <value>
        /// The state of the test.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public TestResultState TestState { get; set; }

        /// <summary>
        /// Gets or sets the time taken.
        /// </summary>
        /// <value>
        /// The time taken.
        /// </value>
        public TimeSpan TimeTaken { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>
        /// The stack trace.
        /// </value>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the create date of the test.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        /// <value>
        /// The name of the assembly.
        /// </value>
        public string AssemblyName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultModel"/> class.
        /// </summary>
        public TestResultModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultModel"/> class from a <see cref="TestResult"/> object.
        /// </summary>
        /// <param name="result">The test result to populate this model with.</param>
        public TestResultModel(TestResult result)
        {
            TestName = result.FullName;
            NameSpace = result.NameSpace;
            TestState = result.State;
            TimeTaken = result.TimeTaken;
            ErrorMessage = result.ErrorMessage;
            StackTrace = result.StackTrace;
            CreateDate = result.CreateDateTime;
            AssemblyName = result.Assembly;
        }

        #endregion
    }
}
