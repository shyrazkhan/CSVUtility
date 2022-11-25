using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSVUtility.Core.Parser
{
    public static class ParserHelper
    {
        public static bool TryReadCsvRow(this TextReader source, out IEnumerable<string> row, StringBuilder quoteBuffer = null)
        {
            row = ReadCsvRow(source, quoteBuffer);
            return row != null;
        }

        public static IEnumerable<string> ReadCsvRow(this TextReader source, StringBuilder quoteBuffer = null)
        {
            var line = source.ReadLine();
            if (line == null) return null;
            return ReadCsvCells(source, line, quoteBuffer);
        }

        private static IEnumerable<string> ReadCsvCells(TextReader source, string line, StringBuilder buf)
        {
            for (var pos = 0; line?.Length >= pos;)
                yield return ReadCsvCell(source, ref line, ref pos, ref buf);
        }

        private static string ReadCsvCell(TextReader source, ref string line, ref int pos, ref StringBuilder buf)
        {
            var len = line.Length;
            if (pos >= len)
            {
                pos = len + 1;
                return "";
            }

            if (line[pos] != '"')
            {
                var end = line.IndexOf(',', pos);
                var head = pos;
                // Last cell in this row.
                if (end < 0)
                {
                    pos = len + 1;
                    return line.Substring(head);
                }
                // Empty cell.
                if (end == pos)
                {
                    pos++;
                    return "";
                }
                pos = end + 1;
                return line.Substring(head, end - head);
            }

            // Quoted cell.
            if (buf == null)
                buf = new StringBuilder();
            else
                buf.Clear();
            var start = ++pos; // Drop opening quote.
            while (true)
            {
                var end = pos < len
                    ? line.IndexOf('"', pos)
                    : -1;
                var next = end + 1;

                // End of line.  Append and read next line.
                if (end < 0)
                {
                    buf.Append(line, start, len - start);
                    if ((line = source.ReadLine()) == null)
                        return buf.ToString();
                    buf.Append('\n');
                    start = pos = 0; len = line.Length;
                    // End of cell.
                }
                else if (next == len || line[next] == ',') // Two double quotes.
                {
                    pos = end + 2;
                    return buf.Append(line, start, end - start).ToString();
                }
                else if (line[next] == '"') // One double quote not followed by EOL or comma.
                {
                    buf.Append(line, start, end - start + 1);
                    pos = start = end + 2;
                }
                else
                    pos++;
            }
        }
    }
}
