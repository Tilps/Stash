using System;
using System.Collections.Generic;
using System.Text;

namespace LOTRODefrag
{
    class StatusProgress
    {
        private string name;

        public string Name
        {
            get { return name; }
        }
        private string newStatus;

        public string NewStatus
        {
            get { return newStatus; }
        }

        public StatusProgress(string name, string newStatus)
        {
            this.name = name;
            this.newStatus = newStatus;
        }
    }
}
