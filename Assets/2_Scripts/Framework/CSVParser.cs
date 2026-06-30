using System.Collections.Generic;
using System.Text;

public static class CSVParser
{
    public static string[] ParseLine(string line)
    {
        List<string> fields = new List<string>();
        StringBuilder currentField = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        fields.Add(currentField.ToString());

        return fields.ToArray();
    }

    public static string[] SplitLines(string csvText)
    {
        List<string> lines = new List<string>();
        StringBuilder currentLine = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < csvText.Length; i++)
        {
            char c = csvText[i];

            if (c == '"')
            {

                if (i + 1 < csvText.Length && csvText[i + 1] == '"')
                {
                    currentLine.Append("\"\"");
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                    currentLine.Append(c);
                }
            }
            else if (c == '\n' && !inQuotes)
            {
                string line = currentLine.ToString().TrimEnd('\r');
                lines.Add(line);
                currentLine.Clear();
            }
            else if (c == '\r')
            {
                continue;
            }
            else
            {
                currentLine.Append(c);
            }
        }

        if (currentLine.Length > 0)
        {
            string line = currentLine.ToString().TrimEnd('\r');
            lines.Add(line);
        }

        return lines.ToArray();
    }
}
