using NationalInstruments.VisaNS;
using System;

namespace RigolLib
{
    public class Oscilloscope : IDisposable
    {
        private readonly string resource;
        private readonly ResourceManager resourceManager;
        private MessageBasedSession session = null;

        internal Oscilloscope(ResourceManager resourceManager, string resource)
        {
            this.resource = resource;
            this.resourceManager = resourceManager;
        }

        double xincrement, xorigin, xreference, yincrement, yorigin, yreference;
        bool raw;

        public Waveform GetWaveform(int channel, bool raw)
        {
            SendCommand(":WAV:FORMat BYTE");
            SendCommand(":WAV:SOURce CHAN" + channel);
            if (raw)
            {
                SendCommand(":WAV:MODE RAW");
                SendCommand(":WAV:STARt 1");
                SendCommand(":WAV:STOP " + QueryString(":ACQuire:MDEPth?"));
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

        public Waveform QueryWaveform()
        {
            if (raw)
                SendCommand(":STOP");

            byte[] wavData = QueryBytes(":WAV:DATA?");
            Waveform waveform = new Waveform("s", "V");

            int wavDataLength = int.Parse(System.Text.Encoding.ASCII.GetString(wavData, 6, 4));

            //Skip first 11 bytes, those are info data
            for (int x = WAV_DATA_X_START; x < wavDataLength + WAV_DATA_X_START; x++)
            {
                waveform.AddPoint(
                    (((double)(x - WAV_DATA_X_START)) - xorigin - xreference) * xincrement,
                    (((double)wavData[x]) - yorigin - yreference) * yincrement
                );
            }

            if (raw)
                SendCommand(":RUN");

            return waveform;
        }

        private MessageBasedSession GetSession()
        {
            if (session == null)
                session = (MessageBasedSession)resourceManager.Open(resource);
            session.DefaultBufferSize = 2048;
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
            return GetSession().Query(query);
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
