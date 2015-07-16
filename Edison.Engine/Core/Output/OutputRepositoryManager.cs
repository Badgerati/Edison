/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Engine.Core.Output
{
    public static class OutputRepositoryManager
    {

        public static IOutputRepository Get(OutputType type)
        {
            switch (type)
            {
                case OutputType.Csv:
                    return CsvOutputRepository.Instance;

                case OutputType.Json:
                    return JsonOutputRepository.Instance;

                case OutputType.Txt:
                    return TxtOutputRepository.Instance;

                case OutputType.Xml:
                    return XmlOutputRepository.Instance;
            }

            return JsonOutputRepository.Instance;
        }

    }
}
