using NationalInstruments.Visa;
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

        protected string QuerySession(string query, int bufferSize = -1)
        {
            if (bufferSize > 0)
            {
                session.FormattedIO.WriteBufferSize = bufferSize;
                session.FormattedIO.ReadBufferSize = bufferSize;
            }
            var io = GetSession().FormattedIO;
            io.WriteLine(query);
            var res = io.ReadLine();
            if (bufferSize > 0)
            {
                io.WriteBufferSize = DEFAULT_BUFFER_SIZE;
                io.ReadBufferSize = DEFAULT_BUFFER_SIZE;
            }
            return res;
        }

        protected byte[] QuerySessionBytes(string query, int bufferSize = -1)
        {
            var io = GetSession().FormattedIO;
            if (bufferSize > 0)
            {
                io.WriteBufferSize = bufferSize;
                io.ReadBufferSize = bufferSize;
            }
            io.WriteLine(query);
            var res = io.ReadLineBinaryBlockOfByte();
            if (bufferSize > 0)
            {
                io.WriteBufferSize = DEFAULT_BUFFER_SIZE;
                io.ReadBufferSize = DEFAULT_BUFFER_SIZE;
            }
            return res;
        }

        internal BaseDevice(ResourceManager resourceManager, string resource)
        {
            this.resource = resource;
            this.resourceManager = resourceManager;

            this.Idendity = QuerySession("*IDN?");
        }
        
            
        protected MessageBasedSession GetSession()
        {
            if (session == null)
                session = (MessageBasedSession)resourceManager.Open(resource);
            session.FormattedIO.WriteBufferSize = DEFAULT_BUFFER_SIZE;
            session.FormattedIO.ReadBufferSize = DEFAULT_BUFFER_SIZE;
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
                GetSession().FormattedIO.WriteLine(command);
            }
        }

        protected string QueryString(string query, int bufferSize = DEFAULT_BUFFER_SIZE)
        {
            string ret;
            lock (communiationLock)
            {
                ret = QuerySession(query, bufferSize);
            }
            return ret.Remove(ret.Length - 1);
        }

        protected byte[] QueryBytes(string query, int bufferSize = DEFAULT_BUFFER_SIZE)
        {
            byte[] ret;
            lock (communiationLock)
            {
                ret = QuerySessionBytes(query, bufferSize);
            }
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
