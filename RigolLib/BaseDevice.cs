using NationalInstruments.VisaNS;
using System;

namespace RigolLib
{
    public class BaseDevice : IDisposable
    {
        private readonly string resource;
        private readonly ResourceManager resourceManager;
        private MessageBasedSession session = null;

        internal BaseDevice(ResourceManager resourceManager, string resource)
        {
            this.resource = resource;
            this.resourceManager = resourceManager;
        }
        
            
        protected MessageBasedSession GetSession()
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

        protected void SendCommand(string command)
        {
            GetSession().Write(command);
        }

        protected string QueryString(string query)
        {
            string ret = GetSession().Query(query);
            return ret.Remove(ret.Length - 1);
        }

        protected byte[] QueryBytes(string query)
        {
            byte[] response = GetSession().Query(System.Text.Encoding.ASCII.GetBytes(query));
            byte[] ret = new byte[response.Length - 1];
            Buffer.BlockCopy(response, 0, ret, 0, ret.Length);
            return ret;
        }

        protected double ParseScientific(string arg)
        {
            return Double.Parse(arg, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }

        protected double QueryScientific(string query)
        {
            return ParseScientific(QueryString(query));
        }
    }
}
