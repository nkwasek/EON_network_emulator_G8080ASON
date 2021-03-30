using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CableCloud
{
    class SNPP
    {
        public string address;
        public bool status;
        public int port;
        public SNPP(string add)
        {
            address = add;
            status = true;
            string[] ipSegments = address.Split('.');
            string Dadd = ipSegments[0] + "." + ipSegments[1] + ".0.0";
            if(Dadd == Program.D1address)
            {
                port = Program.D1Port;
            }
            else if(Dadd == Program.D2address)
            {
                port = Program.D2Port;
            }
            else
            {
                Console.WriteLine(Dadd);
            }
        }
    }
}
