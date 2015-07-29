using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DatOptimize
{
    public class CSV : IDisposable
    {
        public CSV(string fileName)
        {
            reader = File.OpenText(fileName);
        }
        private TextReader reader;

        public string[] GetRow()
        {
            List<string> row = new List<string>();
            StringBuilder curBuild = new StringBuilder();
            bool inQuotes = false;
            bool freshExit = false;
            do
            {
                string current = reader.ReadLine();
                if (current == null)
                    return null;
                foreach (char c in current)
                {
                    if (c == '"')
                    {
                        if (!inQuotes && freshExit)
                        {
                            inQuotes = true;
                            freshExit = false;
                            curBuild.Append("\"");
                        }
                        else
                        {
                            if (inQuotes)
                                freshExit = true;
                            inQuotes = !inQuotes;
                            continue;
                        }
                    }
                    else if (c == ',' && !inQuotes)
                    {
                        row.Add(curBuild.ToString());
                        curBuild.Length = 0;
                    }
                    else
                    {
                        curBuild.Append(c);
                    }
                    if (freshExit)
                        freshExit = false;
                }
                if (inQuotes)
                {
                    curBuild.Append(Environment.NewLine);
                }
            } while (inQuotes);
            row.Add(curBuild.ToString());
            return row.ToArray();
        }

        #region IDisposable Members

        public void Dispose()
        {
            reader.Close();
        }

        #endregion
    }
}
