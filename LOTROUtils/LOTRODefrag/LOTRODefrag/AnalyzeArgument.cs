using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace LOTRODefrag
{
    class AnalyzeArgument
    {
        public AnalyzeArgument(DataGridViewRowCollection dataGridViewRowCollection)
        {
            files = new List<string>(dataGridViewRowCollection.Count);
            foreach (DataGridViewRow row in dataGridViewRowCollection)
            {
                files.Add((string)row.Cells[1].Value);
            }
        }

        public IEnumerable<string> Files { get { return files; } }
        private List<string> files;
    }
}
