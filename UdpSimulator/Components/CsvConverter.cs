using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UdpSimulator.Components
{
    /// <summary>
    /// CSVファイル-SimulationObject 変換.
    /// </summary>
    internal class CsvConverter
    {
        private static readonly string filename = $"{Environment.CurrentDirectory}\\UdpSimulator.csv";

        private static readonly string[] headers = { "データ", "計測桁", "計測値" };

        private static readonly string patternDataLength = @"データ\(表示数:([0-9]+)\)";

        private static readonly Encoding encoding = Encoding.UTF8;

        private static readonly int MaxDataLength = 255;

        public int DataLength { get; private set; } = 0;

        public IEnumerable<SimulationObject> Items { get; private set; } = new List<SimulationObject>();

        public bool Load()
        {
            if (!File.Exists(filename))
            {
                return false;
            }

            IEnumerable<string> csvHeaderItems;
            var csvItems = new List<IEnumerable<string>>();

            using (var sr = new StreamReader(filename, encoding))
            {
                csvHeaderItems = SplitCSV(sr.ReadLine());

                while (!sr.EndOfStream)
                {
                    csvItems.Add(SplitCSV(sr.ReadLine()));
                }
            }

            var regex = new Regex(patternDataLength);
            var match = regex.Match(csvHeaderItems.First());

            if (int.TryParse(match.Groups[1].Value, out int result))
            {
                this.DataLength = result <= MaxDataLength ? result : MaxDataLength;
            }

            this.Items = csvItems.Select(_ => ConvertItem(_)).ToArray();

            return true;
        }

        public void Save(IEnumerable<SimulationObject> items)
        {
            using (var sw = new StreamWriter(filename, false, encoding))
            {
                string JoinLine(IEnumerable<string> vs)
                {
                    var convert = vs.Select(_ => $"\"{_.Replace("\"", "\"\"")}\"");

                    return string.Join(",", convert).Trim(',');
                }

                var val = headers.ToArray();
                val[0] = $"{headers[0]}(表示数:{this.DataLength})";

                sw.WriteLine(JoinLine(val));

                foreach (var a in items)
                {
                    sw.WriteLine(ConvertCsv(a));
                }
            }
        }

        private static string ConvertCsv(SimulationObject item)
        {
            var replacedName = item.Name != null ? $"\"{item.Name.Replace("\"", "\"\"")}\"" : $"\"\"";

            return $"{replacedName},{item.Digits},{item.Value}";
        }

        private static SimulationObject ConvertItem(IEnumerable<string> vs)
        {
            var item = new SimulationObject();
            var array = vs.ToArray();

            item.Name = array[0];

            if (array.Length < 2)
            {
                return item;
            }

            if (byte.TryParse(array[1], out byte resultDigits))
            {
                item.Digits = resultDigits;
            }

            if (array.Length > 2)
            {
                if (double.TryParse(array[2], out double resultValue))
                {
                    item.Value = resultValue;
                }
            }

            return item;
        }

        private static IEnumerable<string> SplitCSV(string line)
        {
            var options =
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.Multiline |
                RegexOptions.IgnoreCase;

            var pattern = "(?: ^|,)(\\\"(?:[^\\\"]+|\\\"\\\")*\\\"|[^,]*)";
            var regex = new Regex(pattern, options);

            foreach (Match a in regex.Matches(line))
            {
                var trim = a.Groups[0].Value.Trim(',').Trim();
                if (trim.Any())
                {
                    var first = trim.First();
                    var last = trim.Last();
                    if ((first == '"' && last == '"') || (first != '"' && last != '"'))
                    {
                        yield return trim.Trim('"').Trim('"').Trim().Replace("\"\"", "\"");
                    }
                }
            }
        }
    }
}
