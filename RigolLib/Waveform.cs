using System.Collections.Generic;

namespace RigolLib
{
    public class Waveform
    {
        public static readonly Waveform EMPTY = new Waveform("", "", new Point[0]);

        public readonly string XUnit, YUnit;
        public readonly Point[] Points;

        public class Point
        {
            public readonly double X;
            public readonly double Y;
            public readonly double RawX;
            public readonly double RawY;

            public Point(double x, double y, double rawX, double rawY)
            {
                X = x;
                Y = y;
                RawX = rawX;
                RawY = rawY;
            }
        }

        public Waveform(string xunit, string yunit, Point[] points)
        {
            XUnit = xunit;
            YUnit = yunit;
            Points = points;
        }
    }
}
