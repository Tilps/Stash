using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace LOTRODefrag
{
    class DefragArgument
    {
        public DefragArgument(System.Windows.Forms.DataGridViewRowCollection dataGridViewRowCollection, bool internalDefrag)
        {
            files = new List<string>(dataGridViewRowCollection.Count);
            foreach (DataGridViewRow row in dataGridViewRowCollection)
            {
                if ((bool)row.Cells[0].Value)
                {
                    files.Add((string)row.Cells[1].Value);
                }
            }
            this.internalDefrag = internalDefrag;
        }

        public bool InternalDefrag { get { return internalDefrag; } }
        private bool internalDefrag;

        public IEnumerable<string> Files { get { return files; } }
        private List<string> files;
    }
}
