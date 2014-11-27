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
            public readonly double RawX;
            public readonly double RawY;

            internal Point(double x, double y, double rawX, double rawY)
            {
                X = x;
                Y = y;
                RawX = rawX;
                RawY = rawY;
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
