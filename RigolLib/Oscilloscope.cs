using NationalInstruments.Visa;
using System;
using System.Collections.Generic;

namespace RigolLib
{
    public class Oscilloscope : BaseDevice
    {
        //const int WAV_DATA_X_START = 0;
        const int MAX_WAVEFORM_QUERY_SIZE = 1000000;

        readonly double horizontalScales; // MSO1000Z/DS1000Z

        double xincrement, xorigin, xreference, yincrement, yorigin, yreference;
        bool raw, single;
        long mdepth;
        string channel = "";

        internal Oscilloscope(ResourceManager resourceManager, string resource, int horizontalScales) : base(resourceManager, resource)
        {
            this.horizontalScales = horizontalScales;
        }

        public void Run()
        {
            SendCommand(":RUN");
        }

        public void SetWaveformConfig(string channel, bool raw = false, bool single = false, bool fast = true)
        {
            lock (communiationLock)
            {
                if (this.raw != raw)
                    fast = false;

                this.raw = raw;
                this.single = single;

                if (this.channel != channel || !fast)
                    SendCommand(":WAV:SOURce " + channel);

                if (!fast)
                {
                    string mdepthStr = QueryString(":ACQuire:MDEPth?");
                    if (mdepthStr == "AUTO")
                        mdepth = (long)(QueryScientific(":ACQuire:SRATe?") * QueryScientific(":TIMebase:MAIN:SCALe?") * horizontalScales);
                    else
                        mdepth = long.Parse(mdepthStr);

                    SendCommand(":WAV:FORMat BYTE");

                    if (raw)
                    {
                        SendCommand(":WAV:MODE RAW");
                    }
                    else
                    {
                        SendCommand(":WAV:MODE NORM");
                        SendCommand(":WAV:STARt 1");
                        SendCommand(":WAV:STOP 1200");
                    }

                    string[] preamble = QueryString(":WAVeform:PREamble?").Split(',');

                    xincrement = ParseScientific(preamble[4]);
                    xorigin = ParseScientific(preamble[5]);
                    xreference = ParseScientific(preamble[6]);
                    yincrement = ParseScientific(preamble[7]);
                    yorigin = ParseScientific(preamble[8]);
                    yreference = ParseScientific(preamble[9]);
                }
            }
        }

        private long AddWaveformData(List<Waveform.Point> points, long offset, int waveformSize)
        {
            byte[] wavData = QueryBytes(":WAV:DATA?", waveformSize + BaseDevice.DEFAULT_BUFFER_SIZE);
            if (wavData == null)
            {
                throw new NullReferenceException();
            }

            for (long x = 0; x < wavData.Length; x++)
            {
                double y = wavData[x];
                points.Add(new Waveform.Point(
                    (((double)(x + offset)) - xorigin - xreference) * xincrement,
                    (y - yorigin - yreference) * yincrement,
                    x,
                    y
                ));
            }

            return wavData.Length;
        }

        public Waveform QueryWaveform()
        {
            List<Waveform.Point> points = new List<Waveform.Point>();

            lock (communiationLock)
            {
                if (raw)
                {
                    SendCommand(":STOP");
                    if (single)
                        SendCommand(":SINGle");

                    long currentPos = 0;
                    while (currentPos < mdepth)
                    {
                        SendCommand(":WAV:STARt " + (currentPos + 1));
                        SendCommand(":WAV:STOP " + Math.Min(currentPos + MAX_WAVEFORM_QUERY_SIZE, mdepth));
                        currentPos += AddWaveformData(points, currentPos, MAX_WAVEFORM_QUERY_SIZE);
                    }

                    SendCommand(":RUN");
                }
                else
                {
                    if (single)
                        SendCommand(":SINGle");
                    AddWaveformData(points, 0, 1200);
                }
            }

            return new Waveform("s", "V", points.ToArray());
        }
    }
}
