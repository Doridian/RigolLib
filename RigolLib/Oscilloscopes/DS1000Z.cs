namespace RigolLib.Oscilloscopes
{
    public class DS1000Z : Oscilloscope
    {
        public DS1000Z(string ip, int port) : base(ip, port, 12) { }
    }
}
