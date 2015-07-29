using System;
using System.Collections.Generic;
using System.Text;

namespace AppCatch
{
    class WindowEntry
    {
        public IntPtr HWnd;
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }
}
