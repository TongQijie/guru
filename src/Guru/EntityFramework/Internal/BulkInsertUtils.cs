using Guru.ExtensionMethod;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Guru.EntityFramework.Internal
{
    internal static class BulkInsertUtils
    {
        public static void CheckIfValidataion(BulkInsertData data)
        {
            if (data == null || data.Values == null)
            {
                throw new Exception("data cannot be null");
            }

            if (!data.TableName.HasValue())
            {
                throw new Exception("table name cannot be empty");
            }

            if (!data.ParameterNames.HasLength())
            {
                throw new Exception("parameter names cannot be empty.");
            }

            if (data.Values.GetLength(1) != data.ParameterNames.Length)
            {
                throw new Exception("number of values cannot match number of parameters");
            }
        }

        public static string RebuildCommandText(string commandText, BulkInsertData data)
        {
            var match = Regex.Match(commandText,
                $"(^|\\s+)INSERT\\s+(INTO\\s+)?{data.TableName}\\s*.*?\\s*VALUES\\s*\\(\\s?(?<parameters>.+?)\\s?\\)\\s*(;|$)+",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!match.Success)
            {
                throw new Exception("command text is invalid");
            }

            var parameters = match.Groups["parameters"].Captures[0].Value.SplitByChar(',');

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Values.GetLength(0); i++)
            {
                stringBuilder.Append("(" + string.Join(",", parameters.Select(x => x + i)) + "),");
            }

            return Regex.Replace(commandText,
                $"(?<header>(^|\\s+)INSERT\\s+(INTO\\s+)?{data.TableName}\\s*.*?\\s*VALUES\\s*)(?<parameters>\\(\\s?.+?\\s?\\))(?<footer>\\s*(;|$)+)",
                m => m.Groups["header"].Captures[0].Value + stringBuilder.ToString().TrimEnd(',') + m.Groups["footer"].Captures[0].Value,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
    }
}
