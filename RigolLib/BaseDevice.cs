using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace RigolLib
{
    public class BaseDevice : IDisposable
    {
        internal const int DEFAULT_BUFFER_SIZE = 2048;

        private readonly IPEndPoint endPoint;
        private TcpClient session = null;
        private NetworkStream networkStream = null;
        private StreamReader networkStreamReader = null;
        private StreamWriter networkStreamWriter = null;

        public readonly string Idendity;

        protected readonly object communiationLock = new object();

        protected string QuerySession(string query)
        {
            WriteLine(query);
            return ReadLine();
        }

        protected byte[] QuerySessionBytes(string query, int len)
        {
            WriteLine(query);
            byte[] res = new byte[len];
            networkStreamReader.BaseStream.Read(res, 0, len);
            if (networkStreamReader.Read() == 0x0a)
            {
                throw new Exception("Wrong line length on binary query");
            }
            return res;
        }

        internal BaseDevice(string ip, int port)
        {
            this.endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            this.Idendity = QuerySession("*IDN?");
        }
        
            
        protected void GetSession()
        {
            if (session == null)
            {
                session = new TcpClient();
                session.Connect(this.endPoint);
                networkStream = session.GetStream();
                networkStreamReader = new StreamReader(networkStream);
                networkStreamWriter = new StreamWriter(networkStream);
            }
            session.ReceiveBufferSize = DEFAULT_BUFFER_SIZE;
            session.SendBufferSize = DEFAULT_BUFFER_SIZE;
        }

        protected void WriteLine(string line)
        {
            GetSession();
            networkStreamWriter.WriteLine(line);
            networkStreamWriter.Flush();
        }

        protected string ReadLine()
        {
            GetSession();
            return networkStreamReader.ReadLine();
        }

        public void Dispose()
        {
            if (session != null)
            {
                session.Close();
            }
            if (networkStream != null)
            {
                networkStream.Dispose();
            }
            if (networkStreamReader != null)
            {
                networkStreamReader.Dispose();
            }
            if (networkStreamWriter != null)
            {
                networkStreamWriter.Dispose();
            }
        }

        protected void SendCommand(string command)
        {
            lock (communiationLock)
            {
                WriteLine(command);
            }
        }

        protected string QueryString(string query)
        {
            string ret;
            lock (communiationLock)
            {
                ret = QuerySession(query);
            }
            return ret;
        }

        protected byte[] QueryBytes(string query, int length)
        {
            byte[] ret;
            lock (communiationLock)
            {
                ret = QuerySessionBytes(query, length);
            }
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
