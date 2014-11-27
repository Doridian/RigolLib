using RigolLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigolScopeLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Oscilloscope oscilloscope = DeviceEnumerator.FindScopes().First();
            Waveform waveform = oscilloscope.GetWaveform(1, false);
            foreach(KeyValuePair<double, double> point in waveform.GetPoints())
            {
                Console.Out.WriteLine(point.Key + waveform.xunit + " => " + point.Value + waveform.yunit);
            }
            Console.In.ReadLine();
        }
    }
}
