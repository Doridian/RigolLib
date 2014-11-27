using NationalInstruments.VisaNS;

namespace RigolLib.Oscilloscopes
{
    public class DS1000Z : Oscilloscope
    {
        internal DS1000Z(ResourceManager resourceManager, string resource) : base(resourceManager, resource, 12) { }
    }
}
