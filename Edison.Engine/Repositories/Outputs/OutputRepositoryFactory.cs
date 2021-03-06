﻿/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Core.Enums;
using Edison.Engine.Repositories.Interfaces;

namespace Edison.Engine.Repositories.Outputs
{
    public static class OutputRepositoryFactory
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

                case OutputType.Dot:
                    return DotOutputRepository.Instance;

                case OutputType.None:
                    return NoneOutputRepository.Instance;

                case OutputType.Markdown:
                    return MarkdownOutputRepository.Instance;

                case OutputType.Html:
                    return HtmlOutputRepository.Instance;
            }

            return JsonOutputRepository.Instance;
        }

    }
}
