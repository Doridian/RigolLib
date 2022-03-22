using RigolLib.Oscilloscopes;
using System;
using System.Windows.Forms;

namespace RigolGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormOscilloscope(new DS1000Z("1.3.3.7", 5555)));
        }
    }
}
