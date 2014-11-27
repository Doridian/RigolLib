using NationalInstruments.VisaNS;
using System;

namespace RigolLib
{
    public class Oscilloscope : IDisposable
    {
        private readonly string resource;
        private readonly ResourceManager resourceManager;
        private MessageBasedSession session = null;

        const double NUMBER_HORIZONTAL_SCALES = 12; // MSO1000Z/DS1000Z

        internal Oscilloscope(ResourceManager resourceManager, string resource)
        {
            this.resource = resource;
            this.resourceManager = resourceManager;
        }

        double xincrement, xorigin, xreference, yincrement, yorigin, yreference;
        bool raw;
        long mdepth;

        public Waveform GetWaveform(int channel, bool raw)
        {
            SendCommand(":WAV:FORMat BYTE");
            SendCommand(":WAV:SOURce CHAN" + channel);

            string mdepthStr = QueryString(":ACQuire:MDEPth?");
            Console.Out.WriteLine(QueryString(":TIMebase:MAIN:SCALe?"));
            if (mdepthStr == "AUTO")
                mdepth = (long)(QueryScientific(":ACQuire:SRATe?") * QueryScientific(":TIMebase:MAIN:SCALe?") * NUMBER_HORIZONTAL_SCALES);
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

        private MessageBasedSession GetSession()
        {
            if (session == null)
                session = (MessageBasedSession)resourceManager.Open(resource);
            session.DefaultBufferSize = 1000000 + 8192;
            return session;
        }

        public void Dispose()
        {
            if (session != null)
                session.Dispose();
        }

        private void SendCommand(string command)
        {
            GetSession().Write(command);
        }

        private string QueryString(string query)
        {
            return GetSession().Query(query).Trim();
        }

        private byte[] QueryBytes(string query)
        {
            return GetSession().Query(System.Text.Encoding.ASCII.GetBytes(query));
        }

        private double ParseScientific(string arg)
        {
            return Double.Parse(arg, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }

        private double QueryScientific(string query)
        {
            return ParseScientific(QueryString(query));
        }
    }
}
