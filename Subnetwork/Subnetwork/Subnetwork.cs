using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subnetwork
{
    class Subnetwork
    {
        public LRM lrm;
        public RoutingConntroller RC;
        public string subID;

        public Subnetwork(string id)
        {
            subID = id;
            lrm = new LRM(subID);
            RC = new RoutingConntroller();

        }
    }
}
