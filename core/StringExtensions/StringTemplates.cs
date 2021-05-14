using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Glow.Core.StringExtensions
{
    public static class StringTemplatesExtensions
    {
        /// <summary>
        /// Replaces all occurences of the form {<identifier>:format} in the given string
        /// 'Identifier' is an argument that specifies the locations in the string where the given DateTime
        /// parameter should be inserted.
        /// Example:
        /// 'It is {MyDate:yyyy/MM/dd} today'.RpleaceDateTimes(date: DateTime.Parse(2021/10/20), identifier: "MyDate")
        /// will produce the string 'It is 2021/10/20 today'
        /// </summary>
        /// <param name="self">The template string in which all templates will be replaced</param>
        /// <param name="date">The datetime value that should be inserted</param>
        /// <param name="identifier">The identifier of the template value</param>
        /// <returns></returns>
        public static string ReplaceDatetimes(this string self, DateTime date, string identifier = "DateTime")
        {
            var globalSearchPattern = @$"{{{identifier}:.*?}}";
            var getFormatPattern = @$"(?<={{{identifier}:).*?(?=}})";

            var input = self;
            var matches = Regex.Matches(input, globalSearchPattern).ToList();

            var result = input;
            foreach (Match v in matches)
            {
                var val = v.Value;
                Match innerMatch = Regex.Match(val, getFormatPattern);
                var innerValue = innerMatch.Value;
                result = result.Replace(v.Value, date.ToString(innerValue));
            }

            return result;
        }

        /// <summary>
        /// Replaces all occurrences of the form {<identifier>:Name} with a link with 'Name'
        /// as the inner text and the parameter url as the links href/target
        /// Example:
        /// 'click {Registration:here} to get to the registration'.ReplaceEntityLink(identifier: "Registration",
        /// url: "https://example.com") would produce
        /// 'click <a href="https://example.com>here</a> to get to the registration.'
        /// </summary>
        /// <param name="self"></param>
        /// <param name="identifier"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ReplaceEntityLink(this string self, string identifier, string url)
        {
            var globalSearchPattern = @$"{{{identifier}:.*?}}";
            var getFormatPattern = @$"(?<={{{identifier}:).*?(?=}})";

            var input = self;
            var matches = Regex.Matches(input, globalSearchPattern).ToList();

            var result = input;

            foreach (Match v in matches)
            {
                var val = v.Value;
                Match innerMatch = Regex.Match(val, getFormatPattern);
                var innerValue = innerMatch.Value;
                result = result.Replace(v.Value,
                    url == null
                        ? $@"<b>## could not replace {{{identifier}}} ##</b>"
                        : $@"<a href=""{url}"">{innerValue}</a>");
            }

            return result;
        }
    }
}