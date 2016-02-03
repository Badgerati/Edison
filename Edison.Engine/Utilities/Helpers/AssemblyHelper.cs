using Edison.Framework;
using System;
using Edison.Engine.Utilities.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Edison.Engine.Utilities.Helpers
{
    public static class AssemblyHelper
    {

        public static Assembly GetAssembly(string path)
        {
            return Assembly.LoadFile(Path.GetFullPath(path));
        }

    }
}
