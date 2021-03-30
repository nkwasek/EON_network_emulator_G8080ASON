using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Subnetwork
{
    class SubnetworkServer
    {
        public bool cosiek;
        private List<Connection> connections = new List<Connection>();
        private int currentId;
        private int currentIndex;
        private static int listenPort = SubnetworkConfig.LPort;
        private static int NCCPort = SubnetworkConfig.SPort;
        private static int NextDomainPort = SubnetworkConfig.NDport;
        private static int MCPort = SubnetworkConfig.MCport;
        private static int CCPort = SubnetworkConfig.CCport;
        private static string EXPort = SubnetworkConfig.externalAddress;
        private static string EXPort2 = SubnetworkConfig.externalAddress2;
        private static UdpClient listener;
        public List<Subnetwork> ChildSubnetworks = new List<Subnetwork>();
        private static int CallingPCC;
        private static int CalledPCC;
        public static string BeginAddress;
        public static string CalledAddress;
        public string CallingAddress;
        private static int DemandedCapacity;
        private static LRM MainLRM;
        public RoutingConntroller MainRC;
        public string ExternalAddress;
        public string SendAddress;
        public List<string> path = new List<string>(); //zapisane jako id
        public List<string> pathAddresses = new List<string>(); //zapisane jako adresy (polaczenia w domenie)
        public List<string> pathAddress = new List<string>(); //pojedyncze adresy (polaczenia w domenie)
        public List<string> pathAll = new List<string>(); //wszystkie sciezki dla danego polaczenia
        public List<string> pathSort = new List<string>(); //posortowana lista pathAll, do wysylania do CableCloud
        public List<string> snppSort = new List<string>(); //posortowana lista SNPP na podtsawie pathSort
        public int LastSlotIndex;
        public int slots;
        public int length;
        private bool reserved;
        private string txt;
        private string gap = "\n                                         ";
        
        public void SNPPSort()
        {
            /*
            Console.WriteLine("=====TEST=====");
            for(int i=0;i<pathSort.Count;i++)
            {
                Console.WriteLine(pathSort[i]);
            }
            */
            snppSort = new List<string>();
            string pomAd;
            if(pathSort[0].Split(' ')[0].Split(':')[0] == pathSort[1].Split(' ')[0].Split(':')[0])
            {
                snppSort.Add(pathSort[0].Split(' ')[1]);
                snppSort.Add(pathSort[0].Split(' ')[0]);
                snppSort.Add(pathSort[1].Split(' ')[1]);
                snppSort.Add(pathSort[1].Split(' ')[0]);
                pomAd = pathSort[1].Split(' ')[0].Split(':')[0];
                for(int i=2;i<pathSort.Count;i++)
                {
                    if(pomAd == pathSort[i].Split(' ')[0].Split(':')[0])
                    {
                        snppSort.Add(pathSort[i].Split(' ')[0]);
                        snppSort.Add(pathSort[i].Split(' ')[1]);
                        pomAd = pathSort[i].Split(' ')[1].Split(':')[0];
                    }
                    else if(pomAd == pathSort[i].Split(' ')[1].Split(':')[0])
                    {
                        snppSort.Add(pathSort[i].Split(' ')[1]);
                        snppSort.Add(pathSort[i].Split(' ')[0]);
                        pomAd = pathSort[i].Split(' ')[0].Split(':')[0];
                    }
                }
            }
            else if(pathSort[0].Split(' ')[0].Split(':')[0] == pathSort[1].Split(' ')[1].Split(':')[0])
            {
                snppSort.Add(pathSort[0].Split(' ')[1]);
                snppSort.Add(pathSort[0].Split(' ')[0]);
                snppSort.Add(pathSort[1].Split(' ')[1]);
                snppSort.Add(pathSort[1].Split(' ')[0]);
                pomAd = pathSort[1].Split(' ')[0].Split(':')[0];
                for (int i = 2; i < pathSort.Count; i++)
                {
                    if (pomAd == pathSort[i].Split(' ')[0].Split(':')[0])
                    {
                        snppSort.Add(pathSort[i].Split(' ')[0]);
                        snppSort.Add(pathSort[i].Split(' ')[1]);
                        pomAd = pathSort[i].Split(' ')[1].Split(':')[0];
                    }
                    else if (pomAd == pathSort[i].Split(' ')[1].Split(':')[0])
                    {
                        snppSort.Add(pathSort[i].Split(' ')[1]);
                        snppSort.Add(pathSort[i].Split(' ')[0]);
                        pomAd = pathSort[i].Split(' ')[0].Split(':')[0];
                    }
                }
            }
            else if (pathSort[0].Split(' ')[1].Split(':')[0] == pathSort[1].Split(' ')[0].Split(':')[0])
            {
                snppSort.Add(pathSort[0].Split(' ')[0]);
                snppSort.Add(pathSort[0].Split(' ')[1]);
                snppSort.Add(pathSort[1].Split(' ')[0]);
                snppSort.Add(pathSort[1].Split(' ')[1]);
                pomAd = pathSort[1].Split(' ')[1].Split(':')[0];
                for (int i = 2; i < pathSort.Count; i++)
                {
                    if (pomAd == pathSort[i].Split(' ')[0].Split(':')[0])
                    {
                        snppSort.Add(pathSort[i].Split(' ')[0]);
                        snppSort.Add(pathSort[i].Split(' ')[1]);
                        pomAd = pathSort[i].Split(' ')[1].Split(':')[0];
                    }
                    else if (pomAd == pathSort[i].Split(' ')[1].Split(':')[0])
                    {
                        snppSort.Add(pathSort[i].Split(' ')[1]);
                        snppSort.Add(pathSort[i].Split(' ')[0]);
                        pomAd = pathSort[i].Split(' ')[0].Split(':')[0];
                    }
                }
            }
            else if (pathSort[0].Split(' ')[1].Split(':')[0] == pathSort[1].Split(' ')[1].Split(':')[0])
            {
                snppSort.Add(pathSort[0].Split(' ')[0]);
                snppSort.Add(pathSort[0].Split(' ')[1]);
                snppSort.Add(pathSort[1].Split(' ')[1]);
                snppSort.Add(pathSort[1].Split(' ')[0]);
                pomAd = pathSort[1].Split(' ')[0].Split(':')[0];
                for (int i = 2; i < pathSort.Count; i++)
                {
                    if (pomAd == pathSort[i].Split(' ')[0].Split(':')[0])
                    {
                        snppSort.Add(pathSort[i].Split(' ')[0]);
                        snppSort.Add(pathSort[i].Split(' ')[1]);
                        pomAd = pathSort[i].Split(' ')[1].Split(':')[0];
                    }
                    else if (pomAd == pathSort[i].Split(' ')[1].Split(':')[0])
                    {
                        snppSort.Add(pathSort[i].Split(' ')[1]);
                        snppSort.Add(pathSort[i].Split(' ')[0]);
                        pomAd = pathSort[i].Split(' ')[0].Split(':')[0];
                    }
                }
            }
        }
        public void PathSort()
        {
            pathSort = new List<string>();
            List<bool> tmpl = new List<bool>();
            for (int m = 0; m < pathAll.Count; m++)
            {
                tmpl.Add(false);
            }
            string tmpA = BeginAddress.Split(':')[0];
            for (int k = 0; k < pathAll.Count; k++)
            {
                for (int l = 0; l < pathAll.Count; l++)
                {
                    if (tmpA == pathAll[l].Split(' ')[0].Split(':')[0] && tmpl[l] == false)
                    {
                        //Console.WriteLine(pathAll[l]);
                        tmpA = pathAll[l].Split(' ')[1].Split(':')[0];
                        tmpl[l] = true;
                        pathSort.Add(pathAll[l]);
                        break;
                    }
                    else if (tmpA == pathAll[l].Split(' ')[1].Split(':')[0] && tmpl[l] == false)
                    {
                        //Console.WriteLine(pathAll[l]);
                        tmpA = pathAll[l].Split(' ')[0].Split(':')[0];
                        tmpl[l] = true;
                        pathSort.Add(pathAll[l]);
                        break;
                    }
                }
            }

        }

        public void FindConnections(string address)
        {
            string msg = string.Empty;
            string space = "\n                                ";

            foreach (Connection c in connections)
            {
                c.MakePATH();
                int i = 0;
                foreach (string addr in c.PATH)
                {
                    string nodeAddr = addr.Split(':')[0];
                    if (nodeAddr == address)
                    {
                        string lastIndex = (c.startIndex + c.slots).ToString();
                        if (i == 0)
                        {
                            msg += space + addr;
                            i++;
                        }
                        else
                        {
                            msg += " " + addr + " " + c.startIndex + "-" + (Convert.ToInt32(lastIndex)-1);
                        }
                    }
                }
            }
            send(MCPort, msg);
        }
        public void SubnetworkConnection(int n, string a, string b)
        {
            Program.ReturnLog("[CCParent] -> CCChild "+(n+1)+" : ConnectionRequest "+ a.Split(':')[0] + " " + b.Split(':')[0] + " REQUEST");
            Program.ReturnLog("[CCChild " + (n + 1)+ "] -> RC " + (n + 1) + " : RouteTableQuery " + a.Split(':')[0] + " " + b.Split(':')[0] + " REQUEST");
            //Console.WriteLine(a + " " + b);
            int tmp = ChildSubnetworks[n].lrm.getIndex(a.Split(':')[0]);
            int tmp2 = ChildSubnetworks[n].lrm.getIndex(b.Split(':')[0]);
            connections[currentIndex].subAddress.Add(a + " " + b);
            //Console.WriteLine(tmp + " " + tmp2);
            path = new List<string>();
            path = ChildSubnetworks[n].RC.RouteTableQuery(tmp, tmp2);
            if(ChildSubnetworks[n].RC.path!=int.MaxValue)
            {
                length += ChildSubnetworks[n].RC.path;
                ChildSubnetworks[n].RC.lastSlotIndex = LastSlotIndex;
                txt = String.Empty;
                txt = "[RC " + (n + 1) + "] -> CCChild " + (n + 1) + " : RouteTableQuery RESPONSE:";
                for (int k = 0; k < path.Count; k++)
                {
                    txt += gap;
                    txt += ChildSubnetworks[n].lrm.Visualisation(Convert.ToInt32(path[k].Split(' ')[0]), Convert.ToInt32(path[k].Split(' ')[1]));
                    pathAll.Add(ChildSubnetworks[n].lrm.getAddress(Convert.ToInt32(path[k].Split(' ')[0]), Convert.ToInt32(path[k].Split(' ')[1])));

                }

                Program.ReturnLog(txt);
                if (tmp == tmp2)
                {
                    Console.WriteLine("                                            SNPP: " + a);
                    Console.WriteLine("                                            SNPP: " + b);
                }
                Program.ReturnLog("[CCChild " + (n + 1) + "] -> LRM " + (n + 1) + " : LinkConnectionRequest REQUEST");
                if (tmp == tmp2)
                {
                    Console.WriteLine($"[{ DateTime.Now}]" + " [LRM " + (n + 1) + "] -> CCChild " + (n + 1) + " : LinkConnectionRequest CONFIRMED");
                }
                else
                    Console.WriteLine($"[{ DateTime.Now}]" + " [LRM " + (n + 1) + "] -> CCChild " + (n + 1) + " : LinkConnectionRequest CONFIRMED: ");
                ChildSubnetworks[n].lrm.LinkConnectionRequest(path, slots, ChildSubnetworks[n].RC.lastSlotIndex, currentId);
                ChildSubnetworks[n].RC.lastSlotIndex = ChildSubnetworks[n].RC.lastSlotIndex + slots;
                Program.ReturnLog("[CCChild " + (n + 1) + "] -> CCParent : ConnectionRequest " + a.Split(':')[0] + " " + b.Split(':')[0] + " SET");
            }
            else
            {
                txt = String.Empty;
                txt = "[RC " + (n + 1) + "] -> CCChild " + (n + 1) + " : RouteTableQuery RESPONSE";
                Program.ReturnLog(txt);
                Program.ReturnLog("[CCChild " + (n + 1) + "] -> CCParent : ConnectionRequest " + a.Split(':')[0] + " " + b.Split(':')[0] + " UNABLE");
                reserved = false;
            }
            
        }

        public void SubnetworkDeallocation(int n, string a, string b)
        {
            Program.ReturnLog("[CCParent] -> CCChild " + (n + 1) + " : ConnectionRequest " + a.Split(':')[0] + " " + b.Split(':')[0] + " RELEASE");
            int tmp = ChildSubnetworks[n].lrm.getIndex(a.Split(':')[0]);
            int tmp2 = ChildSubnetworks[n].lrm.getIndex(b.Split(':')[0]);
            Program.ReturnLog("[CCChild " + (n + 1) + "] -> LRM " + (n + 1) + " : LinkConnectionDeallocation REQUEST");
            if (tmp == tmp2)
            {
                Console.WriteLine($"[{ DateTime.Now}]" + " [LRM " + (n + 1) + "] -> CCChild " + (n + 1) + " : LinkConnectionDeallocation RESPONSE");
            }
            else
                Console.WriteLine($"[{ DateTime.Now}]" + " [LRM " + (n + 1) + "] -> CCChild " + (n + 1) + " : LinkConnectionDeallocation RESPONSE: ");
            ChildSubnetworks[n].lrm.LinkConnectionDeallocation(currentId);
            Program.ReturnLog("[CCChild " + (n + 1) + "] -> CCParent : ConnectionRequest " + a.Split(':')[0] + " " + b.Split(':')[0] + " RELEASED");
        }
        public void Alloc(int n, string brAddress)
        {
            int indexpom = LastSlotIndex;
            ChildSubnetworks[n].RC.LocalTopology(ChildSubnetworks[n].lrm.LocalTopology());
            Program.ReturnLog("[LRM " + (n + 1) + "] -> CCChild " + (n + 1) + " : LinkConnectionAllocation RESPONSE: " + brAddress);
            Program.ReturnLog("[CCChild " + (n + 1) + "] -> CCParent " + (n + 1) + " : LinkConnectionAllocation RESPONSE: " + brAddress);
            List<int> conn = new List<int>();
            conn = ChildSubnetworks[n].lrm.ConnectionsId(brAddress);
            ChildSubnetworks[n].lrm.DeallocateSlots(conn);
            for (int i = 0; i < conn.Count; i++)
            {
                reserved = true;
                for (int j = 0; j < connections.Count; j++)
                {
                    if (connections[j].id == conn[i])
                    {
                        currentIndex = j;
                    }
                }
                CallingAddress = connections[currentIndex].srcAddress;
                CalledAddress = connections[currentIndex].dstAddress;
                BeginAddress = connections[currentIndex].beginAddress;
                DemandedCapacity = connections[currentIndex].capacity;
                SendAddress = connections[currentIndex].sendAddress;

                connections[currentIndex].cleanAllPath(brAddress);
                pathAll = connections[currentIndex].pathAll;

                currentId = connections[currentIndex].id;
                slots = connections[currentIndex].slots;
                LastSlotIndex = connections[currentIndex].startIndex;
                pathAddress = connections[currentIndex].pathAddress;
                string a = connections[currentIndex].findSubnetworkConn2(brAddress);
                SubnetworkConnection(n, a.Split(' ')[0], a.Split(' ')[1]);
                if (reserved == false)
                {
                    string msg = "depath " + CallingAddress + " " + CalledAddress + " " + connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                    Thread.Sleep(10);
                    send(CCPort, msg);
                    Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionDeallocation REQUEST");
                    Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionDeallocation RESPONSE: ");
                    MainLRM.LinkConnectionDeallocation(currentId);
                    for (int q = 0; q < pathAddress.Count; q++)
                    {
                        for (int w = q + 1; w < pathAddress.Count; w++)
                        {
                            if (ChildSubnetworks[0].lrm.findAddress(pathAddress[q], pathAddress[w]))
                            {
                                SubnetworkDeallocation(0, pathAddress[q], pathAddress[w]);

                            }
                            if (ChildSubnetworks[1].lrm.findAddress(pathAddress[q], pathAddress[w]))
                            {
                                SubnetworkDeallocation(1, pathAddress[q], pathAddress[w]);
                            }
                        }

                    }
                    Thread.Sleep(10);
                    string text = "ConnectionRequest " + CallingAddress + " " + SendAddress + " " + CalledAddress + " " + DemandedCapacity + " UNABLE";
                    send(NCCPort, text);
                    Program.ReturnLog("[CCParent] -> NCC : " + text);
                    connections.RemoveAt(currentIndex);
                }
                else if (reserved == true)
                {
                    connections[currentIndex].pathAll = pathAll;
                    PathSort();
                    Console.WriteLine("Wyznaczona sciezka: ");
                    for (int m = 0; m < pathSort.Count; m++)
                    {
                        Console.WriteLine(pathSort[m]);
                    }
                    SNPPSort();
                    string msg2 = "repath ";
                    for (int m = 0; m < snppSort.Count; m++)
                    {
                        msg2 += snppSort[m] + " ";

                    }
                    msg2 += connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                    //Console.WriteLine(msg);
                    Thread.Sleep(10);
                    send(CCPort, msg2);
                }

            }
            LastSlotIndex = indexpom;
        }
        public void DeallocAndAlloc(int n, string brAddress)
        {
            int indexpom = LastSlotIndex;
            ChildSubnetworks[n].RC.LocalTopology(ChildSubnetworks[n].lrm.LocalTopology());
            Program.ReturnLog("[LRM " + (n + 1) + "] -> CCChild " + (n + 1) + " : LinkConnectionDeallocation RESPONSE: " + brAddress);
            Program.ReturnLog("[CCChild " + (n + 1) + "] -> CCParent " + (n + 1) + " : LinkConnectionDeallocation RESPONSE: " + brAddress);
            List<int> conn = new List<int>();
            conn = ChildSubnetworks[n].lrm.ConnectionsId(brAddress);
            ChildSubnetworks[n].lrm.DeallocateSlots(conn);
            for (int i = 0; i < conn.Count; i++)
            {
                cosiek = true;
                reserved = true;
                for (int j = 0; j < connections.Count; j++)
                {
                    if (connections[j].id == conn[i])
                    {
                        currentIndex = j;
                    }
                }
                CallingAddress = connections[currentIndex].srcAddress;
                CalledAddress = connections[currentIndex].dstAddress;
                BeginAddress = connections[currentIndex].beginAddress;
                DemandedCapacity = connections[currentIndex].capacity;
                SendAddress = connections[currentIndex].sendAddress;
                
                connections[currentIndex].cleanAllPath(brAddress);
                pathAll = connections[currentIndex].pathAll;
                
                currentId = connections[currentIndex].id;
                slots = connections[currentIndex].slots;
                LastSlotIndex = connections[currentIndex].startIndex;
                pathAddress = connections[currentIndex].pathAddress;
                string a = connections[currentIndex].findSubnetworkConn2(brAddress);
                SubnetworkConnection(n, a.Split(' ')[0], a.Split(' ')[1]);
                if (reserved == false)
                {
                    string msg = "depath " + CallingAddress + " " + CalledAddress + " " + connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                    Thread.Sleep(10);
                    send(CCPort, msg);
                    Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionDeallocation REQUEST");
                    Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionDeallocation RESPONSE: ");
                    MainLRM.LinkConnectionDeallocation(currentId);
                    for (int q = 0; q < pathAddress.Count; q++)
                    {
                        for (int w = q + 1; w < pathAddress.Count; w++)
                        {
                            if (ChildSubnetworks[0].lrm.findAddress(pathAddress[q], pathAddress[w]))
                            {
                                SubnetworkDeallocation(0, pathAddress[q], pathAddress[w]);

                            }
                            if (ChildSubnetworks[1].lrm.findAddress(pathAddress[q], pathAddress[w]))
                            {
                                SubnetworkDeallocation(1, pathAddress[q], pathAddress[w]);
                            }
                        }

                    }
                    Thread.Sleep(10);
                    string text = "ConnectionRequest " + CallingAddress + " " + SendAddress + " " + CalledAddress + " " + DemandedCapacity + " UNABLE";
                    send(NCCPort, text);
                    Program.ReturnLog("[CCParent] -> NCC : " + text);
                    connections.RemoveAt(currentIndex);
                }
                else if(reserved == true)
                {
                    connections[currentIndex].pathAll = pathAll;
                    PathSort();
                    Console.WriteLine("Wyznaczona sciezka: ");
                    for (int m = 0; m < pathSort.Count; m++)
                    {
                        Console.WriteLine(pathSort[m]);
                    }
                    SNPPSort();
                    string msg2 = "repath ";
                    for (int m = 0; m < snppSort.Count; m++)
                    {
                        msg2 += snppSort[m] + " ";

                    }
                    msg2 += connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                    //Console.WriteLine(msg);
                    Thread.Sleep(10);
                    send(CCPort, msg2);
                }
                cosiek = false;

            }
            LastSlotIndex = indexpom;
        }

        public void StartListener(string path1, string path2, string path3)
        {
            MainRC = new RoutingConntroller();
            MainLRM = new LRM("0");
            ChildSubnetworks.Add(new Subnetwork("1"));
            ChildSubnetworks.Add(new Subnetwork("2"));
            MainLRM.LoadConfig(path1);
            ChildSubnetworks[0].lrm.LoadConfig(path2);
            ChildSubnetworks[1].lrm.LoadConfig(path3);
            MainRC.LocalTopology(MainLRM.LocalTopology());
            ChildSubnetworks[0].RC.LocalTopology(ChildSubnetworks[0].lrm.LocalTopology());
            ChildSubnetworks[1].RC.LocalTopology(ChildSubnetworks[1].lrm.LocalTopology());
            try
            {
                while (true)
                {
                    listener = new UdpClient(listenPort);
                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

                    Console.WriteLine("Listening..");
                    byte[] bytes = listener.Receive(ref groupEP);
                    string message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    string[] splittedMessage = message.Split(' ');
                    if (splittedMessage[0] != "NetworkTopology" && splittedMessage[0] != "repair" && splittedMessage[0] != "break" && splittedMessage[0] != "NodeConnections")
                    {
                        Program.ReturnLog("[CCParent] Received message: " + message + " From: NCC");
                    }
                    if (splittedMessage[0] == "NetworkTopology")
                    {
                        LastSlotIndex = Convert.ToInt32(splittedMessage[1]);
                        MainRC.lastSlotIndex = Convert.ToInt32(splittedMessage[1]);
                        Program.ReturnLog("[RC 0] Received message: " + message+ " From: RC 0");
                    }
                    else

                   
                    if(splittedMessage[0] == "repair")
                    {
                        string brAddress = splittedMessage[1];
                        if (MainLRM.turnOn(brAddress))
                        {
                            MainRC.LocalTopology(MainLRM.LocalTopology());
                            int indexpom = LastSlotIndex;
                            Program.ReturnLog("[LRM 0] -> CCParent : LinkConnectionAllocation RESPONSE: " + brAddress);
                            List<int> conn = new List<int>();
                            conn = MainLRM.ConnectionsId(brAddress);
                            MainLRM.DeallocateSlots(conn);
                            ChildSubnetworks[0].lrm.DeallocateSlots(conn);
                            ChildSubnetworks[1].lrm.DeallocateSlots(conn);
                            for (int i = 0; i < conn.Count; i++)
                            {
                                reserved = true;
                                currentIndex = i;
                                CallingAddress = connections[currentIndex].srcAddress;
                                CalledAddress = connections[currentIndex].dstAddress;
                                BeginAddress = connections[currentIndex].beginAddress;
                                DemandedCapacity = connections[currentIndex].capacity;
                                SendAddress = connections[currentIndex].sendAddress;

                                currentId = connections[currentIndex].id;
                                slots = connections[currentIndex].slots;
                                LastSlotIndex = connections[currentIndex].startIndex;

                                Program.ReturnLog("[CCParent] -> RC 0 : RouteTableQuery " + CallingAddress + " " + BeginAddress + " " + CalledAddress + " REQUEST");
                                int tmp = MainLRM.address.IndexOf(connections[currentIndex].beginAddress);
                                int tmp2 = MainLRM.address.IndexOf(connections[currentIndex].endAddress);
                                //Console.WriteLine(connections[currentIndex].beginAddress + " " + connections[currentIndex].endAddress);
                                pathAddress = new List<string>(); path = new List<string>(); pathAll = new List<string>(); pathAddresses = new List<string>();
                                path = MainRC.RouteTableQuery(MainLRM.idAddress[tmp], MainLRM.idAddress[tmp2]);
                                length = MainRC.path;
                                if (MainRC.path != int.MaxValue)
                                {
                                    reserved = true;
                                    txt = String.Empty;
                                    txt = "[RC] -> CCParent : RouteTableQuery RESPONSE:";
                                    for (int j = 0; j < path.Count; j++)
                                    {
                                        txt += gap;
                                        txt += MainLRM.Visualisation(Convert.ToInt32(path[j].Split(' ')[0]), Convert.ToInt32(path[j].Split(' ')[1]));
                                        pathAddresses.Add(MainLRM.getAddress(Convert.ToInt32(path[j].Split(' ')[0]), Convert.ToInt32(path[j].Split(' ')[1])));
                                        for (int k = 0; k < pathAddresses.Count; k++)
                                        {
                                            Console.WriteLine(pathAddresses[k]);
                                        }


                                    }
                                    pathAll = pathAddresses;
                                    Program.ReturnLog(txt);
                                    Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionRequest REQUEST");
                                }
                                else
                                {
                                    reserved = false;
                                    txt = String.Empty;
                                    txt = "[RC 0] -> CCParent : RouteTableQuery RESPONSE";
                                    Program.ReturnLog(txt);
                                }
                                if (reserved == true)
                                {
                                    Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionRequest CONFIRMED: ");
                                    MainLRM.LinkConnectionRequest(path, slots, LastSlotIndex, currentId);
                                    MainLRM.LinkConnectionRequest(slots, LastSlotIndex, currentId);
                                }
                                else if (reserved == true)
                                {
                                    reserved = false;
                                    Program.ReturnLog("[LRM 0] -> CCParent : LinkConnectionRequest UNABLE");
                                }
                                SendAddress = BeginAddress;
                                if (reserved == true)
                                {
                                    pathAddress = new List<string>();
                                    for (int k = 0; k < pathAddresses.Count; k++)
                                    {
                                        pathAddress.Add(pathAddresses[k].Split(' ')[0]);
                                        pathAddress.Add(pathAddresses[k].Split(' ')[1]);

                                    }
                                    for (int k = 0; k < pathAddress.Count; k++)
                                    {
                                        for (int l = k + 1; l < pathAddress.Count; l++)
                                        {
                                            if (ChildSubnetworks[0].lrm.findAddress(pathAddress[k], pathAddress[l]))
                                            {
                                                SubnetworkConnection(0, pathAddress[k], pathAddress[l]);

                                            }
                                            if (ChildSubnetworks[1].lrm.findAddress(pathAddress[k], pathAddress[l]))
                                            {
                                                SubnetworkConnection(1, pathAddress[k], pathAddress[l]);
                                            }
                                        }
                                    }
                                    if (reserved == false)
                                    {
                                        Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionDeallocation REQUEST");
                                        Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionDeallocation RESPONSE: ");
                                        MainLRM.LinkConnectionDeallocation(currentId);
                                        for (int k = 0; k < pathAddress.Count; k++)
                                        {
                                            for (int l = k + 1; l < pathAddress.Count; l++)
                                            {
                                                if (ChildSubnetworks[0].lrm.findAddress(pathAddress[k], pathAddress[l]))
                                                {
                                                    SubnetworkDeallocation(0, pathAddress[k], pathAddress[l]);

                                                }
                                                if (ChildSubnetworks[1].lrm.findAddress(pathAddress[k], pathAddress[l]))
                                                {
                                                    SubnetworkDeallocation(1, pathAddress[k], pathAddress[l]);
                                                }
                                            }

                                        }
                                    }
                                }
                                if (reserved)
                                {
                                    connections[currentIndex].pathAddress = pathAddress;
                                    connections[currentIndex].pathAddresses = pathAddresses;
                                    for (int k = 0; k < connections.Count; k++)
                                    {
                                        if (connections[k].id == currentId)
                                        {
                                            connections[k].pathAddress = pathAddress;
                                        }
                                    }
                                    if (connections[currentIndex].beginAddress == EXPort2 || connections[currentIndex].endAddress == EXPort2)
                                    {
                                        length -= 40;
                                    }
                                    Console.WriteLine("Dlugosc sciezki: " + length);
                                    Console.WriteLine("Wyznaczona sciezka: ");
                                    connections[currentIndex].pathAll = pathAll;
                                    PathSort();
                                    for (int m = 0; m < pathSort.Count; m++)
                                    {
                                        Console.WriteLine(pathSort[m]);
                                    }
                                    SNPPSort();
                                    string msg = "path ";
                                    for (int m = 0; m < snppSort.Count; m++)
                                    {
                                        msg += snppSort[m] + " ";

                                    }
                                    msg += connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                                    Thread.Sleep(10);
                                    send(CCPort, msg);
                                    Thread.Sleep(10);

                                }
                                else if (!reserved)
                                {
                                    string msg = "depath " + CallingAddress + " " + CalledAddress + " " + connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                                    Thread.Sleep(10);
                                    send(CCPort, msg);
                                    Thread.Sleep(10);
                                    string text = "ConnectionRequest " + CallingAddress + " " + SendAddress + " " + CalledAddress + " " + DemandedCapacity + " UNABLE";
                                    send(NCCPort, text);
                                    Program.ReturnLog("[CCParent] -> NCC : " + text);
                                    connections.RemoveAt(currentIndex);
                                }
                            }
                            LastSlotIndex = indexpom;
                        }

                        else if (ChildSubnetworks[0].lrm.turnOn(brAddress))
                        {
                            Alloc(0, brAddress);
                            
                        }

                        else if (ChildSubnetworks[1].lrm.turnOn(brAddress))
                        {
                            Alloc(1, brAddress);
                        }
                    }
                    else
                    if (splittedMessage[0] == "break")
                    {
                        string brAddress = splittedMessage[1];
                        if (MainLRM.turnOff(brAddress))
                        {
                            MainRC.LocalTopology(MainLRM.LocalTopology());
                            int indexpom = LastSlotIndex;
                            Program.ReturnLog("[LRM 0] -> CCParent : LinkConnectionDeallocation RESPONSE: " + brAddress);
                            List<int> conn = new List<int>();
                            conn = MainLRM.ConnectionsId(brAddress);
                            MainLRM.DeallocateSlots(conn);
                            ChildSubnetworks[0].lrm.DeallocateSlots(conn);
                            ChildSubnetworks[1].lrm.DeallocateSlots(conn);
                            for (int i = 0; i < conn.Count; i++)
                            {
                                reserved = true;
                                currentIndex = i;
                                //connections[currentId].subAddress = new List<string>();
                                CallingAddress = connections[currentIndex].srcAddress;
                                CalledAddress = connections[currentIndex].dstAddress;
                                BeginAddress = connections[currentIndex].beginAddress;
                                DemandedCapacity = connections[currentIndex].capacity;
                                SendAddress = connections[currentIndex].sendAddress;
                                
                                currentId = connections[currentIndex].id;
                                slots = connections[currentIndex].slots;
                                LastSlotIndex = connections[currentIndex].startIndex;
                                
                                Program.ReturnLog("[CCParent] -> RC 0 : RouteTableQuery " + CallingAddress + " " + BeginAddress + " " + CalledAddress + " REQUEST");
                                int tmp = MainLRM.address.IndexOf(connections[currentIndex].beginAddress);
                                int tmp2 = MainLRM.address.IndexOf(connections[currentIndex].endAddress);
                                //Console.WriteLine(connections[currentIndex].beginAddress + " " + connections[currentIndex].endAddress);
                                pathAddress = new List<string>(); path = new List<string>(); pathAll = new List<string>(); pathAddresses = new List<string>();
                                path = MainRC.RouteTableQuery(MainLRM.idAddress[tmp], MainLRM.idAddress[tmp2]);
                                length = MainRC.path;
                                if (MainRC.path != int.MaxValue)
                                {
                                    reserved = true;
                                    txt = String.Empty;
                                    txt = "[RC] -> CCParent : RouteTableQuery RESPONSE:";
                                    for (int j = 0; j < path.Count; j++)
                                    {
                                        txt += gap;
                                        txt += MainLRM.Visualisation(Convert.ToInt32(path[j].Split(' ')[0]), Convert.ToInt32(path[j].Split(' ')[1]));
                                        pathAddresses.Add(MainLRM.getAddress(Convert.ToInt32(path[j].Split(' ')[0]), Convert.ToInt32(path[j].Split(' ')[1])));
                                        /*
                                        for(int k=0;k<pathAddresses.Count;k++)
                                        {
                                            Console.WriteLine(pathAddresses[k]);
                                        }
                                        */

                                    }
                                    pathAll = pathAddresses;
                                    Program.ReturnLog(txt);
                                    Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionRequest REQUEST");
                                }
                                else
                                {
                                    reserved = false;
                                    txt = String.Empty;
                                    txt = "[RC 0] -> CCParent : RouteTableQuery RESPONSE";
                                    Program.ReturnLog(txt);
                                }
                                if (reserved == true)
                                {
                                    Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionRequest CONFIRMED: ");
                                    MainLRM.LinkConnectionRequest(path, slots, LastSlotIndex, currentId);
                                    MainLRM.LinkConnectionRequest(slots, LastSlotIndex, currentId);
                                }
                                else if (reserved == true)
                                {
                                    reserved = false;
                                    Program.ReturnLog("[LRM 0] -> CCParent : LinkConnectionRequest UNABLE");
                                }
                                SendAddress = BeginAddress;
                                if (reserved == true)
                                {
                                    pathAddress = new List<string>();
                                    for (int k = 0; k < pathAddresses.Count; k++)
                                    {
                                        pathAddress.Add(pathAddresses[k].Split(' ')[0]);
                                        pathAddress.Add(pathAddresses[k].Split(' ')[1]);

                                    }
                                    for (int k = 0; k < pathAddress.Count; k++)
                                    {
                                        for (int l = k + 1; l < pathAddress.Count; l++)
                                        {
                                            if (ChildSubnetworks[0].lrm.findAddress(pathAddress[k], pathAddress[l]))
                                            {
                                                SubnetworkConnection(0, pathAddress[k], pathAddress[l]);

                                            }
                                            if (ChildSubnetworks[1].lrm.findAddress(pathAddress[k], pathAddress[l]))
                                            {
                                                SubnetworkConnection(1, pathAddress[k], pathAddress[l]);
                                            }
                                        }
                                    }
                                    if (reserved == false)
                                    {
                                        Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionDeallocation REQUEST");
                                        Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionDeallocation RESPONSE: ");
                                        MainLRM.LinkConnectionDeallocation(currentId);
                                        for (int k = 0; k < pathAddress.Count; k++)
                                        {
                                            for (int l = k + 1; l < pathAddress.Count; l++)
                                            {
                                                if (ChildSubnetworks[0].lrm.findAddress(pathAddress[k], pathAddress[l]))
                                                {
                                                    SubnetworkDeallocation(0, pathAddress[k], pathAddress[l]);

                                                }
                                                if (ChildSubnetworks[1].lrm.findAddress(pathAddress[k], pathAddress[l]))
                                                {
                                                    SubnetworkDeallocation(1, pathAddress[k], pathAddress[l]);
                                                }
                                            }

                                        }
                                    }
                                }
                                if (reserved)
                                {
                                    connections[currentIndex].pathAddress = pathAddress;
                                    connections[currentIndex].pathAddresses = pathAddresses;
                                    for (int k = 0; k < connections.Count; k++)
                                    {
                                        if (connections[k].id == currentId)
                                        {
                                            connections[k].pathAddress = pathAddress;
                                        }
                                    }
                                    if(connections[currentIndex].beginAddress == EXPort2|| connections[currentIndex].endAddress == EXPort2)
                                    {
                                        length -= 40;
                                    }
                                    Console.WriteLine("Dlugosc sciezki: " + length);
                                    Console.WriteLine("Wyznaczona sciezka: ");
                                    connections[currentIndex].pathAll = pathAll;
                                    PathSort();
                                    for (int m = 0; m < pathSort.Count; m++)
                                    {
                                        Console.WriteLine(pathSort[m]);
                                    }
                                    SNPPSort();
                                    string msg = "path ";
                                    for (int m = 0; m < snppSort.Count; m++)
                                    {
                                        msg += snppSort[m] + " ";

                                    }
                                    msg += connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                                    Thread.Sleep(10);
                                    send(CCPort, msg);
                                    Thread.Sleep(10);
                                    
                                }
                                else if (!reserved)
                                {
                                    string msg = "depath " + CallingAddress + " " + CalledAddress + " " + connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                                    Thread.Sleep(10);
                                    send(CCPort, msg);
                                    Thread.Sleep(10);
                                    string text = "ConnectionRequest " + CallingAddress + " " + SendAddress + " " + CalledAddress + " " + DemandedCapacity + " UNABLE";
                                    send(NCCPort, text);
                                    Program.ReturnLog("[CCParent] -> NCC : " + text);
                                    connections.RemoveAt(currentIndex);
                                }
                            }
                            LastSlotIndex = indexpom;
                        }

                        else if (ChildSubnetworks[0].lrm.turnOff(brAddress))
                        {
                            DeallocAndAlloc(0, brAddress);                                         
                        }

                        else if (ChildSubnetworks[1].lrm.turnOff(brAddress))
                        {
                            DeallocAndAlloc(1, brAddress);
                        }

                    }
                    else
                    if (splittedMessage[0] == "NodeConnections")
                    {
                        FindConnections(splittedMessage[1]);
                    }
                    else
                    if (splittedMessage[0] == "ConnectionRequest") // Przykład ConnectionRequesta: "ConnectionRequest x:100 x:100 x:200 [capacity] SETUP/RELEASE"
                    {
                        int tmp, tmp2;
                        bool pom = false;
                        if (splittedMessage.Last() == "RELEASE")
                        {
                            CallingAddress = splittedMessage[1];
                            CalledAddress = splittedMessage[2];
                            for (int i = connections.Count - 1; i >= 0; i--)
                            {
                                if (connections[i].finder(CallingAddress, CalledAddress))
                                {
                                    currentIndex = i;
                                    currentId = connections[i].id;
                                    pathAddress = new List<string>();
                                    pathAddress = connections[i].pathAddress;
                                    string msg = "depath " + CallingAddress + " " + CalledAddress + " " + connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                                    Thread.Sleep(10);
                                    send(CCPort, msg);
                                    connections.Remove(connections[i]);
                                    pom = true;
                                    break;
                                }
                            }
                            Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionDeallocation REQUEST");
                            Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionDeallocation RESPONSE: ");
                            MainLRM.LinkConnectionDeallocation(currentId);
                            for (int i = 0; i < pathAddress.Count; i++)
                            {
                                for (int j = i + 1; j < pathAddress.Count; j++)
                                {
                                    if (ChildSubnetworks[0].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                    {
                                        SubnetworkDeallocation(0, pathAddress[i], pathAddress[j]);

                                    }
                                    if (ChildSubnetworks[1].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                    {
                                        SubnetworkDeallocation(1, pathAddress[i], pathAddress[j]);
                                    }
                                }

                            }                          
                           
                            string text = "ConnectionRequest " + CallingAddress + " " + CalledAddress + " RELEASED";
                            Thread.Sleep(10);
                            send(NCCPort, text);
                            Program.ReturnLog("[CCParent] -> NCC : " + text);
                        }
                        else
                        if (splittedMessage[5] == "SETUP")
                        {
                            CallingPCC = Convert.ToInt32(splittedMessage[1].Split(':')[1]);
                            CalledPCC = Convert.ToInt32(splittedMessage[3].Split(':')[1]);
                            DemandedCapacity = Convert.ToInt32(splittedMessage[4]);
                            BeginAddress = splittedMessage[2];
                            CalledAddress = splittedMessage[3];
                            Program.ReturnLog("[CCParent] -> RC 0 : RouteTableQuery "+ splittedMessage[1]+" "+ splittedMessage[2]+" "+ splittedMessage[3]+ " REQUEST");
                            connections.Add(new Connection(splittedMessage[1], splittedMessage[3], Convert.ToInt32(splittedMessage[4])));
                            currentId = connections.Last().id;
                            
                            for (int i = 0; i < connections.Count; i++)
                            {
                                if (connections[i].id == currentId)
                                {
                                    currentIndex = i;
                                }
                            }
                            if ((BeginAddress.Split('.')[1] == CalledAddress.Split('.')[1]) && (BeginAddress.Split('.')[0] == CalledAddress.Split('.')[0]))
                            {
                                connections.Last().startAddress = splittedMessage[1];
                                connections.Last().endAddress = splittedMessage[3];
                                //WEWNATRZDOMENOWE
                                //Console.WriteLine(CalledAddress);
                                tmp = MainLRM.address.IndexOf(BeginAddress);
                                tmp2 = MainLRM.address.IndexOf(CalledAddress);
                                //Console.WriteLine(MainLRM.idAddress[tmp] + " " + MainLRM.idAddress[tmp2]);
                                path = new List<string>();
                                path = MainRC.RouteTableQuery(MainLRM.idAddress[tmp], MainLRM.idAddress[tmp2]);
                                pathAll = new List<string>();
                                if(MainRC.path != int.MaxValue)
                                {
                                    reserved = true;
                                    length = MainRC.path;
                                    slots = MainRC.reqSlots(3, DemandedCapacity);
                                    connections.Last().slots = slots;
                                    connections.Last().startIndex = MainRC.lastSlotIndex;
                                    LastSlotIndex = MainRC.lastSlotIndex;
                                    //Console.WriteLine("Dlugosc: " + length);
                                    txt = String.Empty;
                                    txt = "[RC 0] -> CCParent : RouteTableQuery RESPONSE:";
                                    pathAddresses = new List<string>();
                                    for (int i = 0; i < path.Count; i++)
                                    {
                                        txt += gap;
                                        txt += MainLRM.Visualisation(Convert.ToInt32(path[i].Split(' ')[0]), Convert.ToInt32(path[i].Split(' ')[1]));
                                        pathAddresses.Add(MainLRM.getAddress(Convert.ToInt32(path[i].Split(' ')[0]), Convert.ToInt32(path[i].Split(' ')[1])));

                                    }
                                    pathAll = pathAddresses;
                                    Program.ReturnLog(txt);
                                    Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionRequest REQUEST");
                                }
                                else
                                {
                                    reserved = false;
                                    txt = String.Empty;
                                    txt = "[RC 0] -> CCParent : RouteTableQuery RESPONSE";
                                    Program.ReturnLog(txt);
                                }
                                
                                if (MainRC.lastSlotIndex + slots < 30 && reserved == true)
                                {
                                    Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionRequest CONFIRMED: ");
                                    MainLRM.LinkConnectionRequest(path, slots, MainRC.lastSlotIndex, currentId);
                                    reserved = true;
                                    MainRC.lastSlotIndex = MainRC.lastSlotIndex + slots;
                                    string text = "NetworkTopology " + MainRC.lastSlotIndex + " UPDATE";
                                    Thread.Sleep(10);
                                    send(NextDomainPort, text);
                                    Program.ReturnLog("[RC 0] -> RC 0 : " + text);
                                }
                                else if(reserved == true)
                                {
                                    reserved = false;
                                    Program.ReturnLog("[LRM 0] -> CCParent : LinkConnectionRequest UNABLE");
                                }
                                SendAddress = BeginAddress;
                                if(reserved==true)
                                {
                                    pathAddress = new List<string>();
                                    for (int i = 0; i < pathAddresses.Count; i++)
                                    {
                                        pathAddress.Add(pathAddresses[i].Split(' ')[0]);
                                        pathAddress.Add(pathAddresses[i].Split(' ')[1]);

                                    }
                                    for (int i = 0; i < pathAddress.Count; i++)
                                    {
                                        for (int j = i + 1; j < pathAddress.Count; j++)
                                        {
                                            //Console.WriteLine(pathAddress[i].Split(' ')[0] +" "+ pathAddress[j].Split(' ')[1]);
                                            if (ChildSubnetworks[0].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                            {
                                                SubnetworkConnection(0, pathAddress[i], pathAddress[j]);
                                                
                                            }
                                            if (ChildSubnetworks[1].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                            {
                                                SubnetworkConnection(1, pathAddress[i], pathAddress[j]);
                                            }
                                        }


                                    }
                                    if(reserved==false)
                                    {
                                        Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionDeallocation REQUEST");
                                        Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionDeallocation RESPONSE: ");
                                        MainLRM.LinkConnectionDeallocation(currentId);
                                        for (int i = 0; i < pathAddress.Count; i++)
                                        {
                                            for (int j = i + 1; j < pathAddress.Count; j++)
                                            {
                                                if (ChildSubnetworks[0].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                                {
                                                    SubnetworkDeallocation(0, pathAddress[i], pathAddress[j]);

                                                }
                                                if (ChildSubnetworks[1].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                                {
                                                    SubnetworkDeallocation(1, pathAddress[i], pathAddress[j]);
                                                }
                                            }

                                        }
                                    }
                                }
                                
                            }
                            else
                            {
                                //MIEDZYDOMENOWE
                                tmp = MainLRM.address.IndexOf(BeginAddress);
                                tmp2 = MainLRM.address.IndexOf(ExternalAddress);
                                connections.Last().startAddress = BeginAddress;
                                connections.Last().endAddress = EXPort2;
                                path = new List<string>();
                                path = MainRC.RouteTableQuery(MainLRM.idAddress[tmp], MainLRM.idAddress[tmp2]);
                                pathAll = new List<string>();
                                if (MainRC.path != int.MaxValue)
                                {
                                    reserved = true;
                                    slots = MainRC.reqSlots(2, DemandedCapacity);
                                    LastSlotIndex = MainRC.lastSlotIndex;
                                    connections.Last().slots = slots;
                                    connections.Last().startIndex = MainRC.lastSlotIndex;
                                    txt = String.Empty;
                                    txt = "[RC 0] -> CCParent : RouteTableQuery RESPONSE:";
                                    pathAddresses = new List<string>();

                                    for (int i = 0; i < path.Count; i++)
                                    {
                                        txt += gap;
                                        txt += MainLRM.Visualisation(Convert.ToInt32(path[i].Split(' ')[0]), Convert.ToInt32(path[i].Split(' ')[1]));
                                        pathAddresses.Add(MainLRM.getAddress(Convert.ToInt32(path[i].Split(' ')[0]), Convert.ToInt32(path[i].Split(' ')[1])));

                                    }
                                    pathAll = pathAddresses;
                                    Program.ReturnLog(txt);
                                    Program.ReturnLog("[CCParent] -> LRM 0: LinkConnectionRequest REQUEST");
                                }
                                else
                                {
                                    reserved = false;
                                    txt = String.Empty;
                                    txt = "[RC 0] -> CCParent : RouteTableQuery RESPONSE";
                                    Program.ReturnLog(txt);
                                }

                                   

                                if (MainRC.lastSlotIndex + slots < 30 && reserved == true)
                                {
                                    Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionRequest CONFIRMED: ");
                                    MainLRM.LinkConnectionRequest(path, slots, MainRC.lastSlotIndex, currentId);
                                    MainLRM.LinkConnectionRequest(slots, MainRC.lastSlotIndex, currentId);
                                    pathAddresses.Add(MainLRM.getLink());

                                    reserved = true;
                                    MainRC.lastSlotIndex = MainRC.lastSlotIndex + slots;
                                    
                                }
                                else if (reserved==true)
                                {
                                    reserved = false;
                                    Program.ReturnLog("[LRM 0] -> CCParent : LinkConnectionRequest UNABLE");
                                   
                                }
                                    
                                
                                SendAddress = ExternalAddress;
                                length = MainRC.path;
                                if (reserved == true)
                                {
                                    pathAddress = new List<string>();
                                    for (int i = 0; i < pathAddresses.Count; i++)
                                    {
                                        pathAddress.Add(pathAddresses[i].Split(' ')[0]);
                                        pathAddress.Add(pathAddresses[i].Split(' ')[1]);

                                    }
                                    for (int i = 0; i < pathAddress.Count; i++)
                                    {
                                        for (int j = i + 1; j < pathAddress.Count; j++)
                                        {
                                            //Console.WriteLine(pathAddress[i].Split(' ')[0] +" "+ pathAddress[j].Split(' ')[1]);
                                            if (ChildSubnetworks[0].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                            {
                                                SubnetworkConnection(0, pathAddress[i], pathAddress[j]);

                                            }
                                            if (ChildSubnetworks[1].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                            {
                                                SubnetworkConnection(1, pathAddress[i], pathAddress[j]);
                                            }
                                        }


                                    }
                                    if (reserved == false)
                                    {
                                        Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionDeallocation REQUEST");
                                        Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionDeallocation RESPONSE: ");
                                        MainLRM.LinkConnectionDeallocation(currentId);
                                        for (int i = 0; i < pathAddress.Count; i++)
                                        {
                                            for (int j = i + 1; j < pathAddress.Count; j++)
                                            {
                                                if (ChildSubnetworks[0].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                                {
                                                    SubnetworkDeallocation(0, pathAddress[i], pathAddress[j]);

                                                }
                                                if (ChildSubnetworks[1].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                                {
                                                    SubnetworkDeallocation(1, pathAddress[i], pathAddress[j]);
                                                }
                                            }

                                        }
                                    }
                                }

                            }
                            if(reserved)
                            {
                                connections[currentIndex].pathAddress = pathAddress;
                                connections[currentIndex].pathAddresses = pathAddresses;
                                Console.WriteLine("Dlugosc sciezki: " + length);
                                Console.WriteLine("Wyznaczona sciezka: ");
                                connections[currentIndex].pathAll = pathAll;
                                connections[currentIndex].beginAddress = BeginAddress;
                                connections[currentIndex].sendAddress = SendAddress;
                                PathSort();
                                for (int m = 0; m < pathSort.Count; m++)
                                {
                                    Console.WriteLine(pathSort[m]);
                                }
                                SNPPSort();
                                string msg = "path ";
                                for (int m = 0; m < snppSort.Count; m++)
                                {
                                    msg += snppSort[m] + " ";
                                    
                                }
                                msg += connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                                for (int i = 0; i < connections.Count; i++)
                                {
                                    if (connections[i].id == currentId)
                                    {
                                        connections[i].pathAddress = pathAddress;
                                    }
                                }
                                Thread.Sleep(10);
                                send(CCPort, msg);
                                Thread.Sleep(10);
                                string text = "ConnectionRequest " + BeginAddress + " " + SendAddress + " " + CalledAddress + " " + DemandedCapacity + " " + slots + " " + LastSlotIndex +" SET";
                                send(NCCPort, text);
                                Program.ReturnLog("[CCParent] -> NCC : " + text);
                            }
                            else if(!reserved)
                            {
                                string msg = "depath " + CallingAddress + " " + CalledAddress + " " + connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                                Thread.Sleep(10);
                                send(CCPort, msg);
                                connections.Remove(connections.Last());
                                Thread.Sleep(10);
                                string text = "ConnectionRequest " + BeginAddress + " " + SendAddress + " " + CalledAddress + " " + DemandedCapacity + " UNABLE";
                                send(NCCPort, text);
                                Program.ReturnLog("[CCParent] -> NCC : " + text);
                            }
                        }
                        else if(splittedMessage.Last() == "SETUP")
                        {
                            CallingPCC = Convert.ToInt32(splittedMessage[1].Split(':')[1]);
                            CalledPCC = Convert.ToInt32(splittedMessage[3].Split(':')[1]);
                            DemandedCapacity = Convert.ToInt32(splittedMessage[4]);
                            CallingAddress = splittedMessage[1];
                            BeginAddress = splittedMessage[2];
                            CalledAddress = splittedMessage[3];
                            connections.Add(new Connection(splittedMessage[1], splittedMessage[3], Convert.ToInt32(splittedMessage[4])));
                            currentId = connections.Last().id;
                            for (int i = 0; i < connections.Count; i++)
                            {
                                if (connections[i].id == currentId)
                                {
                                    currentIndex = i;
                                }
                            }
                            Program.ReturnLog("[CCParent] -> RC 0 : RouteTableQuery " + splittedMessage[1] + " " + splittedMessage[2] + " " + splittedMessage[3] + " REQUEST");
                            //WEWNATRZDOMENOWE
                            //Console.WriteLine(CalledAddress);
                            tmp = MainLRM.address.IndexOf(BeginAddress);
                            tmp2 = MainLRM.address.IndexOf(CalledAddress);
                            connections.Last().startAddress = BeginAddress;
                            connections.Last().endAddress = CalledAddress;
                            //Console.WriteLine(MainLRM.idAddress[tmp] + " " + MainLRM.idAddress[tmp2]);
                            path = new List<string>();
                            path = MainRC.RouteTableQuery(MainLRM.idAddress[tmp], MainLRM.idAddress[tmp2]);
                            pathAll = new List<string>();
                            
                            pathAddresses = new List<string>();
                            length = MainRC.path;
                            slots = Convert.ToInt32(splittedMessage[5]);
                            
                            connections.Last().slots = slots;
                            connections.Last().startIndex = Convert.ToInt32(splittedMessage[6]);
                            if (MainRC.path != int.MaxValue)
                            {
                                reserved = true;
                                txt = String.Empty;
                                txt = "[RC] -> CCParent : RouteTableQuery RESPONSE:";
                                for (int i = 0; i < path.Count; i++)
                                {
                                    txt += gap;
                                    txt += MainLRM.Visualisation(Convert.ToInt32(path[i].Split(' ')[0]), Convert.ToInt32(path[i].Split(' ')[1]));
                                    pathAddresses.Add(MainLRM.getAddress(Convert.ToInt32(path[i].Split(' ')[0]), Convert.ToInt32(path[i].Split(' ')[1])));

                                }
                                pathAll = pathAddresses;
                                Program.ReturnLog(txt);
                                LastSlotIndex = Convert.ToInt32(splittedMessage[6]);
                                Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionRequest REQUEST");
                            }
                            else
                            {
                                txt = String.Empty;
                                txt = "[RC 0] -> CCParent : RouteTableQuery RESPONSE";
                                Program.ReturnLog(txt);
                            }
                               
                                
                            if (MainRC.lastSlotIndex + Convert.ToInt32(splittedMessage[5]) < 30 && reserved == true)
                            {
                                Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionRequest CONFIRMED: ");
                                MainLRM.LinkConnectionRequest(path, Convert.ToInt32(splittedMessage[5]), MainRC.lastSlotIndex, currentId);
                                reserved = true;
                                MainRC.lastSlotIndex = MainRC.lastSlotIndex + Convert.ToInt32(splittedMessage[5]);
                            }
                            else if (reserved == true)
                            {
                                reserved = false;
                                Program.ReturnLog("[LRM 0] -> CCParent : LinkConnectionRequest UNABLE");
                            }   
                            SendAddress = BeginAddress;

                            if (reserved == true)
                            {
                                pathAddress = new List<string>();
                                for (int i = 0; i < pathAddresses.Count; i++)
                                {
                                    pathAddress.Add(pathAddresses[i].Split(' ')[0]);
                                    pathAddress.Add(pathAddresses[i].Split(' ')[1]);

                                }
                                for (int i = 0; i < pathAddress.Count; i++)
                                {
                                    for (int j = i + 1; j < pathAddress.Count; j++)
                                    {
                                        //Console.WriteLine(pathAddress[i].Split(' ')[0] +" "+ pathAddress[j].Split(' ')[1]);
                                        if (ChildSubnetworks[0].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                        {
                                            SubnetworkConnection(0, pathAddress[i], pathAddress[j]);

                                        }
                                        if (ChildSubnetworks[1].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                        {
                                            SubnetworkConnection(1, pathAddress[i], pathAddress[j]);
                                        }
                                    }


                                }
                                if (reserved == false)
                                {
                                    Program.ReturnLog("[CCParent] -> LRM 0 : LinkConnectionDeallocation REQUEST");
                                    Console.WriteLine($"[{ DateTime.Now}]" + " [LRM 0] -> CCParent : LinkConnectionDeallocation RESPONSE: ");
                                    MainLRM.LinkConnectionDeallocation(currentId);
                                    for (int i = 0; i < pathAddress.Count; i++)
                                    {
                                        for (int j = i + 1; j < pathAddress.Count; j++)
                                        {
                                            if (ChildSubnetworks[0].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                            {
                                                SubnetworkDeallocation(0, pathAddress[i], pathAddress[j]);

                                            }
                                            if (ChildSubnetworks[1].lrm.findAddress(pathAddress[i], pathAddress[j]))
                                            {
                                                SubnetworkDeallocation(1, pathAddress[i], pathAddress[j]);
                                            }
                                        }

                                    }
                                }
                            }


                            if (reserved)
                            {
                                connections[currentIndex].pathAddress = pathAddress;
                                connections[currentIndex].pathAddresses = pathAddresses;
                                connections[currentIndex].sendAddress = SendAddress;
                                connections[currentIndex].beginAddress = BeginAddress;
                                for (int i=0;i<connections.Count;i++)
                                {
                                    if(connections[i].id == currentId)
                                    {
                                        connections[i].pathAddress = pathAddress;
                                    }
                                }
                                
                                Console.WriteLine("Dlugosc sciezki: " + length);
                                Console.WriteLine("Wyznaczona sciezka: ");
                                connections[currentIndex].pathAll = pathAll;
                                PathSort();
                                for (int m = 0; m < pathSort.Count; m++)
                                {
                                    Console.WriteLine(pathSort[m]);
                                }
                                SNPPSort();
                                string msg = "path ";
                                for (int m = 0; m < snppSort.Count; m++)
                                {
                                    msg += snppSort[m] + " ";

                                }
                                msg += connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                                Thread.Sleep(10);
                                send(CCPort, msg);
                                Thread.Sleep(10);
                                string text = "ConnectionRequest " + CallingAddress + " " + SendAddress + " " + CalledAddress + " " + DemandedCapacity + " " + Convert.ToInt32(splittedMessage[5]) + " " + LastSlotIndex + " SET";
                                send(NCCPort, text);
                                Program.ReturnLog("[CCParent] -> NCC : " + text);
                            }
                            else if (!reserved)
                            {
                                string msg = "depath " + CallingAddress + " " + CalledAddress + " " + connections[currentIndex].startIndex + " " + (connections[currentIndex].startIndex + connections[currentIndex].slots - 1);
                                Thread.Sleep(10);
                                send(CCPort, msg);
                                Thread.Sleep(10);
                                string text = "ConnectionRequest " + BeginAddress + " " + SendAddress + " " + CalledAddress + " " + DemandedCapacity + " UNABLE";
                                send(NCCPort, text);
                                Program.ReturnLog("[CCParent] -> NCC : " + text);
                                connections.Remove(connections.Last());
                            }
                        }
                         
                    } 

                    listener.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }

        public static void send(int port, string text)
        {
            listener.Connect("localhost", port);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(text);
            listener.Send(sendBytes, sendBytes.Length);
        }

       

    }
}
