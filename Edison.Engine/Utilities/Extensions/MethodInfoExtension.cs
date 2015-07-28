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

            if (authorAttributes == default(IEnumerable<AuthorAttribute>) || authorAttributes.Count() == 0)
            {
                return new List<string>();
            }

            return authorAttributes.Select(a => a.Name).ToList();
        }

        public static string GetVersion(this MethodInfo method)
        {
            var versionAttribute = method.GetCustomAttributes().OfType<VersionAttribute>();

            if (versionAttribute == default(IEnumerable<AuthorAttribute>) || versionAttribute.Count() == 0)
            {
                return string.Empty;
            }

            return versionAttribute.First().Value;
        }

    }
}
