using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subnetwork
{
    public class Links
    {
        public static List<int> FirstSNPPAdress = new List<int>();
        public static List<int> SecondSNPPAddress = new List<int>();
        public static List<int> FirstSNPLabel = new List<int>();
        public static List<int> SecondSNPLabel = new List<int>();

        public static void addElement(int fsnppa, int ssnppa, int fsnpl, int ssnpl)
        {
            FirstSNPPAdress.Add(fsnppa);
            SecondSNPPAddress.Add(ssnppa);
            FirstSNPLabel.Add(fsnpl);
            SecondSNPLabel.Add(ssnpl);
        }

    }
}
