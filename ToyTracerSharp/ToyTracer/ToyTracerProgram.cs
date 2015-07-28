using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ToyTracer
{
    static class ToyTracerProgram
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RenderForm());
        }
    }
}