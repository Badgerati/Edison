/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine;
using Edison.Engine.Contexts;
using Edison.Engine.Core.Exceptions;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System;

namespace Edison.Console
{
    public class Program
    {

        #region Repositories

        private static IAppDomainRepository AppDomainRepository
        {
            get { return DIContainer.Instance.Get<IAppDomainRepository>(); }
        }

        #endregion

        #region Exit Codes

        private class ExitCode
        {
            public const int SUCCESS = 0;
            public const int TESTS_FAILED = 1;
            public const int VALIDATED_FAILED = 2;
            public const int ARGUMENT_ERROR = 3;
            public const int UNKNOWN_ERROR = -1;
        }

        #endregion

        #region Main

        public static int Main(string[] args)
        {
            // hook up the event to unload repos on app exit
            AppDomainRepository.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            // create edison instance
            var context = EdisonContext.Create();
            var exitCode = ExitCode.SUCCESS;

            // attempt to parse the passed parameters (from CLI or Edisonfile)
            try
            {
                if (!ParameterParser.Parse(context, args))
                {
                    return ExitCode.SUCCESS;
                }
            }
            catch (ArgumentException aex)
            {
                Logger.Instance.WriteInnerException(aex);
                exitCode = ExitCode.ARGUMENT_ERROR;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteInnerException(ex);
                exitCode = ExitCode.UNKNOWN_ERROR;
            }

            // run all of the tests
            if (exitCode == ExitCode.SUCCESS)
            {
                try
                {
                    var results = context.Run();
                    if (results != null && results.TotalFailedCount > 0)
                    {
                        exitCode = ExitCode.TESTS_FAILED;
                    }
                }
                catch (ValidationException vex)
                {
                    Logger.Instance.WriteError(vex.Message);
                    exitCode = ExitCode.VALIDATED_FAILED;
                }
                catch (Exception ex)
                {
                    Logger.Instance.WriteException(ex);
                    exitCode = ExitCode.UNKNOWN_ERROR;
                }
            }

            #if DEBUG
            {
                System.Console.ReadKey();
            }
            #endif

            // return successful
            return exitCode;
        }

        #endregion

        #region Events

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DIContainer.Instance.Dispose();
        }

        #endregion

    }
}
