using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Subnetwork
{

    class SubnetworkConfig
    {
        public static int LPort { get; set; }
        public static int SPort { get; set; }
        public static int NDport { get; set; }
        public static int MCport { get; set; }
        public static int CCport { get; set; }

        public static string externalAddress { get; set; }
        public static string externalAddress2 { get; set; }


        public static void LoadConfig(string path)
        {
            XmlReader reader = new XmlTextReader(path);

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name.ToString())
                    {
                        case "LPORT":
                            LPort = int.Parse(reader.ReadString());
                            break;
                        case "SPORT":
                            SPort = int.Parse(reader.ReadString());
                            break;  
                        case "NDPORT":
                            NDport = int.Parse(reader.ReadString());
                            break;
                        case "MCPORT":
                            MCport = int.Parse(reader.ReadString());
                            break;
                        case "CCPORT":
                            CCport = int.Parse(reader.ReadString());
                            break;
                        case "external":
                            externalAddress = reader.ReadString();
                            break;
                        case "external2":
                            externalAddress2 = reader.ReadString();
                            break;

                    }
                }
            }
            Program.ReturnLog("Configuration file loaded successfully");
            
        }

    }
}
