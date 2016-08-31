/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine;
using Edison.Engine.Contexts;
using Edison.Engine.Core.Exceptions;
using Edison.Injector;
using System;

namespace Edison.Console
{
    public class Program
    {

        private class ExitCode
        {
            public const int SUCCESS = 0;
            public const int TESTS_FAILED = 1;
            public const int VALIDATED_FAILED = 2;
            public const int ARGUMENT_ERROR = 3;
            public const int UNKNOWN_ERROR = -1;
        }


        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            var context = EdisonContext.Create();

            try
            {
                if (!ParameterParser.Parse(context, args))
                {
                    return ExitCode.ARGUMENT_ERROR;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteInnerException(ex);
                return ExitCode.UNKNOWN_ERROR;
            }

            try
            {
                var results = context.Run();
                if (results != null && results.TotalFailedCount > 0)
                {
                    return ExitCode.TESTS_FAILED;
                }
            }
            catch (ValidationException vex)
            {
                Logger.Instance.WriteError(vex.Message);
                return ExitCode.VALIDATED_FAILED;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteException(ex);
                return ExitCode.UNKNOWN_ERROR;
            }

            return ExitCode.SUCCESS;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DIContainer.Instance.Dispose();
        }

    }
}
