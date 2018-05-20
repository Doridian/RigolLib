using System;
using System.Collections.Generic;

namespace RigolLib
{
    public class Waveform
    {
        public static readonly Waveform EMPTY = new Waveform("", "", new Point[0], 0 , 0 ,0);

        public readonly string XUnit, YUnit;
        public readonly Point[] Points;

        public readonly float R, G, B;

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

        public Waveform(string xunit, string yunit, Point[] points, string channel)
        {
            XUnit = xunit;
            YUnit = yunit;
            Points = points;
            switch (channel)
            {
                case "CHAN1":
                    R = 1.0f;
                    G = 1.0f;
                    B = 0.0f;
                    break;
                case "CHAN2":
                    R = 0.0f;
                    G = 1.0f;
                    B = 1.0f;
                    break;
                case "CHAN3":
                    R = 1.0f;
                    G = 0.0f;
                    B = 1.0f;
                    break;
                case "CHAN4":
                    R = 0.0f;
                    G = 0.5f;
                    B = 1.0f;
                    break;
                case "MATH":
                    R = 1.0f;
                    G = 0.0f;
                    B = 1.0f;
                    break;
                default:
                    Console.Error.WriteLine("Channel " + channel);
                    break;
            }
        }

        public Waveform(string xunit, string yunit, Point[] points, float r, float g, float b)
        {
            XUnit = xunit;
            YUnit = yunit;
            Points = points;
            R = r;
            G = g;
            B = b;
        }
    }
}
