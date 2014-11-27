using NationalInstruments.VisaNS;
using System;

namespace RigolLib
{
    public class Oscilloscope : BaseDevice
    {
        readonly double horizontalScales; // MSO1000Z/DS1000Z

        double xincrement, xorigin, xreference, yincrement, yorigin, yreference;
        bool raw;
        long mdepth;

        internal Oscilloscope(ResourceManager resourceManager, string resource, int horizontalScales) : base(resourceManager, resource)
        {
            this.horizontalScales = horizontalScales;
        }

        public Waveform GetWaveform(int channel, bool raw)
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

            return QueryWaveform();
        }

        const int WAV_DATA_X_START = 11;

        private long AddWaveformData(Waveform waveform, long offset)
        {
            byte[] wavData = QueryBytes(":WAV:DATA?");

            long wavDataLength = long.Parse(System.Text.Encoding.ASCII.GetString(wavData, 2, 9));

            //Skip first 11 bytes, those are info data
            for (long x = WAV_DATA_X_START; x < wavDataLength + WAV_DATA_X_START; x++)
            {
                waveform.AddPoint(
                    (((double)(x - WAV_DATA_X_START + offset)) - xorigin - xreference) * xincrement,
                    (((double)wavData[x]) - yorigin - yreference) * yincrement
                );
            }

            return wavDataLength;
        }

        public Waveform QueryWaveform()
        {
            Waveform waveform = new Waveform("s", "V");

            if (raw)
            {
                SendCommand(":STOP");

                long currentPos = 0;
                while(currentPos < mdepth)
                {
                    SendCommand(":WAV:STARt " + (currentPos + 1));
                    SendCommand(":WAV:STOP " + Math.Min(currentPos + 1000000, mdepth));
                    currentPos += AddWaveformData(waveform, currentPos);
                }

                SendCommand(":RUN");
            }
            else
            {
                AddWaveformData(waveform, 0);
            }

            return waveform;
        }
    }
}
