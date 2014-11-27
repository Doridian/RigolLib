using NationalInstruments.VisaNS;
using System;
using System.Collections.Generic;

namespace RigolLib
{
    public class DeviceEnumerator
    {
        public static IEnumerable<Oscilloscope> FindScopes()
        {
            ResourceManager resourceManager = ResourceManager.GetLocalManager();

            List<Oscilloscope> scopes = new List<Oscilloscope>();

            string[] resources = resourceManager.FindResources("?*INSTR");
            foreach (string resource in resources)
            {
                if (resource.StartsWith("USB") || resource.StartsWith("LAN") || resource.StartsWith("TCP"))
                {
                    try
                    {
                        using (MessageBasedSession session = (MessageBasedSession)resourceManager.Open(resource))
                        {
                            string[] info = session.Query("*IDN?").Split(',');
                            if (info[0] != "RIGOL TECHNOLOGIES")
                                break;
                            switch (info[1])
                            {
                                case "DS1104Z":
                                case "DS1074Z":
                                case "DS1054Z":
                                    scopes.Add(new Oscilloscopes.DS1000Z(resourceManager, resource));
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }
            }

            return scopes;
        }
    }
}
