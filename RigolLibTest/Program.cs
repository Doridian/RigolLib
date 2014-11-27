using RigolLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RigolScopeLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Oscilloscope oscilloscope = DeviceEnumerator.FindScopes().First();
            
            Waveform waveform = oscilloscope.GetWaveform(1, true);

            Console.Out.WriteLine(DateTime.UtcNow);
            long starttime = DateTime.Now.ToFileTimeUtc();
            for (int i = 0; i < 1; i++)
                oscilloscope.QueryWaveform();
            double time = DateTime.Now.ToFileTimeUtc() - starttime;
            Console.Out.WriteLine(DateTime.UtcNow);

            Console.Out.WriteLine((time * 1.0e-9) * 100);

            /*foreach(KeyValuePair<double, double> point in waveform.GetPoints())
            {
                Console.Out.WriteLine(point.Key + waveform.xunit + " => " + point.Value + waveform.yunit);
            }*/

            Console.In.ReadLine();
        }
    }
}
