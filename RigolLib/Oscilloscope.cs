using NationalInstruments.VisaNS;
using System;
using System.Collections.Generic;

namespace RigolLib
{
    public class Oscilloscope : BaseDevice
    {
        const int WAV_DATA_X_START = 11;
        const int MAX_WAVEFORM_QUERY_SIZE = 1000000;

        readonly double horizontalScales; // MSO1000Z/DS1000Z

        double xincrement, xorigin, xreference, yincrement, yorigin, yreference;
        bool raw;
        long mdepth;

        internal Oscilloscope(ResourceManager resourceManager, string resource, int horizontalScales) : base(resourceManager, resource)
        {
            this.horizontalScales = horizontalScales;
        }

        public void Run()
        {
            SendCommand(":RUN");
        }

        public Waveform GetWaveform(int channel, bool raw = false, bool single = false)
        {
            lock (communiationLock)
            {
                SendCommand(":WAV:FORMat BYTE");
                SendCommand(":WAV:SOURce CHAN" + channel);

                string mdepthStr = QueryString(":ACQuire:MDEPth?");
                if (mdepthStr == "AUTO")
                    mdepth = (long)(QueryScientific(":ACQuire:SRATe?") * QueryScientific(":TIMebase:MAIN:SCALe?") * horizontalScales);
                else
                    mdepth = long.Parse(mdepthStr);

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

                this.raw = raw;
                xincrement = ParseScientific(preamble[4]);
                xorigin = ParseScientific(preamble[5]);
                xreference = ParseScientific(preamble[6]);
                yincrement = ParseScientific(preamble[7]);
                yorigin = ParseScientific(preamble[8]);
                yreference = ParseScientific(preamble[9]);

                return QueryWaveform(single);
            }
        }

        private long AddWaveformData(List<Waveform.Point> points, long offset, int waveformSize)
        {
            byte[] wavData = QueryBytes(":WAV:DATA?", waveformSize + BaseDevice.DEFAULT_BUFFER_SIZE);

            long wavDataLength = long.Parse(System.Text.Encoding.ASCII.GetString(wavData, 2, 9));

            //Skip first 11 bytes, those are info data
            for (long x = WAV_DATA_X_START; x < wavDataLength + WAV_DATA_X_START; x++)
            {
                double y = wavData[x];
                points.Add(new Waveform.Point(
                    (((double)(x - WAV_DATA_X_START + offset)) - xorigin - xreference) * xincrement,
                    (y - yorigin - yreference) * yincrement,
                    x,
                    y
                ));
            }

            return wavDataLength;
        }

        public Waveform QueryWaveform(bool single = false)
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
