using NationalInstruments.VisaNS;
using System;

namespace RigolLib
{
    public class BaseDevice : IDisposable
    {
        internal const int DEFAULT_BUFFER_SIZE = 2048;

        private readonly string resource;
        private readonly ResourceManager resourceManager;
        private MessageBasedSession session = null;

        public readonly string Idendity;

        protected readonly object communiationLock = new object();

        internal BaseDevice(ResourceManager resourceManager, string resource)
        {
            this.resource = resource;
            this.resourceManager = resourceManager;

            this.Idendity = GetSession().Query("*IDN?");
        }
        
            
        protected MessageBasedSession GetSession()
        {
            if (session == null)
                session = (MessageBasedSession)resourceManager.Open(resource);
            session.DefaultBufferSize = DEFAULT_BUFFER_SIZE;
            return session;
        }

        public void Dispose()
        {
            if (session != null)
                session.Dispose();
        }

        protected void SendCommand(string command)
        {
            lock (communiationLock)
            {
                GetSession().Write(command);
            }
        }

        protected string QueryString(string query, int bufferSize = DEFAULT_BUFFER_SIZE)
        {
            string ret;
            lock (communiationLock)
            {
                ret = GetSession().Query(query, bufferSize);
            }
            return ret.Remove(ret.Length - 1);
        }

        protected byte[] QueryBytes(string query, int bufferSize = DEFAULT_BUFFER_SIZE)
        {
            byte[] response;
            lock (communiationLock)
            {
                response = GetSession().Query(System.Text.Encoding.ASCII.GetBytes(query), bufferSize);
            }
            byte[] ret = new byte[response.Length - 1];
            Buffer.BlockCopy(response, 0, ret, 0, ret.Length);
            return ret;
        }

        protected double ParseScientific(string arg)
        {
            return Double.Parse(arg, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }

        protected double QueryScientific(string query, int bufferSize = DEFAULT_BUFFER_SIZE)
        {
            return ParseScientific(QueryString(query, bufferSize));
        }
    }
}
