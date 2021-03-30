using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subnetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            try
            {
                SubnetworkConfig.LoadConfig(arguments[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                Environment.ExitCode = 0;
            }
            SubnetworkServer subnetworkServer = new SubnetworkServer();
            subnetworkServer.ExternalAddress = SubnetworkConfig.externalAddress;
            subnetworkServer.StartListener(arguments[2], arguments[3], arguments[4]);
            Console.ReadLine();
        }
        public static void ReturnLog(string log)
        {
            Console.WriteLine($"[{DateTime.Now}]" + " " + log + "\n---------------------");
        }
    }
}
