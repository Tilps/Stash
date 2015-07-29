using System;
using System.Collections.Generic;
using System.Text;

namespace LOTRODefrag
{
    class DatFragsProgress
    {
        public string Name
        {
            get { return fileName; }
        }
        private string fileName;

        public DatFragsProgress(string fileName)
        {
            this.fileName = fileName;
        }
    }
}
