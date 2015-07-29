using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace LOTRODefrag
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!File.Exists("DatExport.dll") || !File.Exists("lotroclient.exe"))
            {
                MessageBox.Show("LOTRODefrag must be copied to LOTRO installation directory and datdefrag must be copied there as well, otherwise LOTRODefrag cannot function. Exiting.");
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
