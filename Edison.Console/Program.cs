/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine;
using Edison.Engine.Contexts;
using Edison.Engine.Repositories.Files;
using System;

namespace Edison.Console
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var context = new EdisonContext();

            try
            {
                if (!ParameterParser.Parse(context, args, new FileRepository()))
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

    }
}
