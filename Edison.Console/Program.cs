/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */
 
using Edison.Engine;
using Edison.Engine.Contexts;
using Edison.Injector;
using System;

namespace Edison.Console
{
    public class Program
    {

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            var context = new EdisonContext();

            try
            {
                if (!ParameterParser.Parse(context, args))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteInnerException(ex);
                return;
            }

            try
            {
                context.Run();
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteException(ex);
                return;
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DIContainer.Instance.Dispose();
        }

    }
}
