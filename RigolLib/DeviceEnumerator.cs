using NationalInstruments.Visa;
using System;
using System.Collections.Generic;

namespace RigolLib
{
    public class DeviceEnumerator
    {
        public static IEnumerable<Oscilloscope> FindScopes()
        {
            ResourceManager resourceManager = new ResourceManager();

            List<Oscilloscope> scopes = new List<Oscilloscope>();

            foreach (string resource in resourceManager.Find("?*INSTR"))
            {
                if (resource.StartsWith("USB") || resource.StartsWith("LAN") || resource.StartsWith("TCP"))
                {
                    try
                    {
                        using (MessageBasedSession session = (MessageBasedSession)resourceManager.Open(resource))
                        {
                            session.FormattedIO.WriteLine("*IDN?");
                            string[] info = session.FormattedIO.ReadLine().Split(',');
                            if (info[0] != "RIGOL TECHNOLOGIES")
                                break;
                            switch (info[1])
                            {
                                case "DS1104Z-S":
                                case "DS1074Z-S":
                                case "DS1054Z-S":
                                case "DS1104Z":
                                case "DS1074Z":
                                case "DS1054Z":
                                case "MSO1104Z-S":
                                case "MSO1074Z-S":
                                case "MSO1054Z-S":
                                case "MSO1104Z":
                                case "MSO1074Z":
                                case "MSO1054Z":
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
