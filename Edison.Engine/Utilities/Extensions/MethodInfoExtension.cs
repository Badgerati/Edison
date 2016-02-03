using Edison.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Engine.Utilities.Extensions
{
    public static class MethodInfoExtension
    {

        public static IList<string> GetAuthors(this MethodInfo method)
        {
            var authorAttributes = method.GetCustomAttributes().OfType<AuthorAttribute>();

            if (authorAttributes == default(IEnumerable<AuthorAttribute>) || !authorAttributes.Any())
            {
                return new List<string>();
            }

            return authorAttributes.Select(a => a.Name).ToList();
        }

        public static string GetVersion(this MethodInfo method)
        {
            var versionAttribute = method.GetCustomAttributes().OfType<VersionAttribute>();

            if (versionAttribute == default(IEnumerable<VersionAttribute>) || !versionAttribute.Any())
            {
                return string.Empty;
            }

            return versionAttribute.First().Value;
        }

        public static ExpectedExceptionAttribute GetExpectedException(this MethodInfo method)
        {
            var exceptionAttribute = method.GetCustomAttributes().OfType<ExpectedExceptionAttribute>();

            if (exceptionAttribute == default(IEnumerable<ExpectedExceptionAttribute>) || !exceptionAttribute.Any())
            {
                return default(ExpectedExceptionAttribute);
            }

            return exceptionAttribute.First();
        }

        public static string GetNamespace(this MethodInfo method)
        {
            return method.DeclaringType.FullName;
        }

        public static string GetFullNamespace(this MethodInfo method)
        {
            return method.DeclaringType.FullName + "." + method.Name;
        }

    }
}
