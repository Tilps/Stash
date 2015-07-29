using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DatOptimize
{
    class DatProfileEntry
    {
        public DatProfileEntry(string filename)
        {
            Filename = filename;
            using (StreamReader reader = File.OpenText(filename))
            {
                reader.ReadLine();
                Name = reader.ReadLine();
                Mode = reader.ReadLine();
            }
        }
        public string Filename { get; private set; }
        public string Name { get; private set; }
        public string Mode { get; private set; }

        public override string ToString()
        {
            return Name + " - " + Mode;
        }
    }
}
