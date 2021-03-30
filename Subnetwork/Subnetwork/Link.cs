using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subnetwork
{
    class Links
    {
        public static List<int> FirstSNPP = new List<int>(); 
        public static List<int> SecondSNPP = new List<int>();
        public static List<int> FirstSNP = new List<int>();
        public static List<int> SecondSNP = new List<int>();

        public static Tuple<int,int> GetSNPs(int fsnpp,int ssnpp)
        {
            for (int i = 0; i < FirstSNPP.Count; i++)
            {
                if(FirstSNPP[i] == fsnpp && SecondSNPP[i] == ssnpp )
                {
                    return new Tuple<int, int>(FirstSNP[i], SecondSNP[i]);
                }
                else if(SecondSNPP[i] == fsnpp && FirstSNPP[i] == ssnpp)
                {
                    return new Tuple<int, int>(SecondSNP[i], FirstSNP[i]);
                }
            }
            return null;
        }

    }
}
