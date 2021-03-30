using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subnetwork
{
    class Connection
    {
        public static int currentId = 0;
        public Connection(string src, string dst, int cap)
        {
            currentId++;
            id = currentId;
            srcAddress = src;
            dstAddress = dst;
            capacity = cap;
        }
        public bool finder(string src, string dst)
        {
            if ((src == srcAddress && dst == dstAddress) || (src == dstAddress && dst == srcAddress))
                return true;
            
            else return false;
        }
        public string findSubnetworkConn(string a)
        {
            for(int i=0;i<subAddress.Count;i++)
            {
                int pom = 0;
                for(int j=0;j<3;j++)
                {
                    if (a.Split('.')[j] == subAddress[i].Split(' ')[0].Split('.')[j] && a.Split('.')[j] == subAddress[i].Split(' ')[1].Split('.')[j])
                        pom++;
                }
                if (pom == 3)
                {
                    return subAddress[i];
                }


            }
            return String.Empty;
        }
        public string findSubnetworkConn2(string a)
        {
            string abc;
            for (int i = 0; i < subAddress.Count; i++)
            {
                int pom = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (a.Split('.')[j] == subAddress[i].Split(' ')[0].Split('.')[j] && a.Split('.')[j] == subAddress[i].Split(' ')[1].Split('.')[j])
                        pom++;
                }
                if (pom == 3 && a.Split('.')[3]!=srcAddress.Split('.')[3] && a.Split('.')[3] != dstAddress.Split('.')[3])
                {
                    abc = subAddress[i];
                    subAddress.RemoveAt(i);
                    return abc;
                }

            }
            return String.Empty;
        }
        public void MakePATH()
        {
            foreach (string pair in pathAll)
            {
                string[] tab = pair.Split(' ');
                PATH.Add(tab[0]);
                PATH.Add(tab[1]);
            }

        }
        public void cleanAllPath(string a)
        {
            for(int i=pathAll.Count-1;i>=0;i--)
            {
                int pom = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (a.Split('.')[j] == pathAll[i].Split(' ')[0].Split('.')[j] && a.Split('.')[j] == pathAll[i].Split(' ')[1].Split('.')[j])
                        pom++;
                }
                if(pom==3 && pathAll[i].Split(' ')[0]!=srcAddress && pathAll[i].Split(' ')[0] != dstAddress)
                {
                    pathAll.RemoveAt(i);
                }
            }
        }
        public List<string> PATH = new List<string>();
        public int id;
        public string srcAddress;
        public string dstAddress;
        public string beginAddress;
        public string sendAddress;
        public string startAddress;
        public string endAddress;
        public int capacity;
        public List<string> pathAddress = new List<string>();
        public List<string> pathAddresses = new List<string>();
        public List<string> pathAll = new List<string>();
        public List<string> subAddress = new List<string>();//do laczenia podsieci
        public int slots;
        public int startIndex;

    }
}
