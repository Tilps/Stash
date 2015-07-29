using System;
using System.Collections.Generic;
using System.Text;

namespace LOTRODefrag
{
    class ExtFragsProgress
    {
        public string Name { get { return fileName; } }
        private string fileName;
        public int NewFragCount { get { return newFragCount; } }
        private int newFragCount;

        public ExtFragsProgress(string fileName, int newFragCount)
        {
            this.fileName = fileName;
            this.newFragCount = newFragCount;
        }
    }
}
