using System.Collections.Generic;

namespace RigolLib
{
    public class Waveform
    {
        public readonly string XUnit, YUnit;
        public readonly Point[] Points;

        public class Point
        {
            public readonly double X;
            public readonly double Y;

            internal Point(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        internal Waveform(string xunit, string yunit, Point[] points)
        {
            XUnit = xunit;
            YUnit = yunit;
            Points = points;
        }
    }
}
