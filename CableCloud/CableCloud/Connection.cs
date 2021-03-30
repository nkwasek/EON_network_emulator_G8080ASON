using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CableCloud
{

    class Connection
    {
        public List<string> SNPPAddresses = new List<string>();
        public int startSlot, endSlot;
        public Connection(int startSlot, int endSlot)
        {
            this.startSlot = startSlot;
            this.endSlot = endSlot;
        }
    }
}
