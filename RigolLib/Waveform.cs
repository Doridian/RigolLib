using System.Collections.Generic;

namespace RigolLib
{
    public class Waveform
    {
        public readonly string xunit, yunit;
        private Dictionary<double, double> points = new Dictionary<double, double>();

        internal Waveform(string xunit, string yunit)
        {
            this.xunit = xunit;
            this.yunit = yunit;
        }

        internal void AddPoint(double time, double value)
        {
            points.Add(time, value);
        }

        public Dictionary<double, double> GetPoints()
        {
            return points;
        }
    }
}
