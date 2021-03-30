using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace CableCloud
{
    class Program
    {
        private static string gap = "                      ";
        private int NPort = 860;
        private int PPort = 1840;
        private int TPort = 2077;
        public static int D1Port = 1111;
        public static int D2Port = 2222;
        private int LPort = 9001;
        UdpClient udpClient;
        UdpClient cloudSender;
        Thread commandThread;
        Thread receiveThread;
        IPEndPoint RemoteIpEndPoint;
        private List<SNPP> SNPPList = new List<SNPP>();
        public static string D1address;
        public static string D2address;
        public List<Connection> connections = new List<Connection>();
        public string Piotrek;
        public string Natalka;
        public string Tomek;
        public void Start()
        {
            ReturnLog("Starting CableCloud...");
            commandThread = new Thread(MakeRequest);
            receiveThread = new Thread(ReceiveMessage);

            try
            {

                RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                commandThread.Start();
                receiveThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Stop()
        {
            string msg = String.Empty;
            udpClient.Close();
            Environment.Exit(-1);
        }

        public void MakeRequest()
        {
            while (true)
            {
                string rl = Console.ReadLine();
                string[] tmp = rl.Split(' ');
                int len = tmp.Length;
                string msg = String.Empty;
                int index = ReturnIndex(tmp[1]);
                if(index == -1)
                {
                    ReturnLog("Wrong address.");
                    continue;
                }

                if (tmp[0] == "break")
                {
                    if (SNPPList[index].status)
                    {
                        ReturnLog("SNPP is now broken.");
                        SendRequest(SNPPList[index].port, rl);
                        SNPPList[index].status = false;
                    }
                    else { ReturnLog("SNPP is already broken."); }
                }
                else if (tmp[0] == "repair")
                {
                    if (!SNPPList[index].status)
                    {
                        ReturnLog("SNPP is now working.");
                        SendRequest(SNPPList[index].port, rl);
                        SNPPList[index].status = true;

                    }
                    else { ReturnLog("SNPP is not broken."); }
                }
                else
                {
                    Console.WriteLine("Unknown command");
                }
            }
        }

        public void SendRequest(int sendPort, string msg)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
            udpClient.Connect("localhost", sendPort);
            udpClient.Send(sendBytes, sendBytes.Length);
        }

        public void SendMSG(int port, string msg)
        {
            cloudSender = new UdpClient(LPort + 1);
            cloudSender.Connect("localhost", port);

            Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
            cloudSender.Connect("localhost", port);
            cloudSender.Send(sendBytes, sendBytes.Length);
            Thread.Sleep(1000);
            cloudSender.Close();
        }

        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    udpClient = new UdpClient(LPort);
                    Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                    string message = Encoding.ASCII.GetString(receiveBytes);
                    //ReturnLog("Received : " + message);
                    string[] splittedMessage = message.Split(' ');
                    int lastIndex = splittedMessage.Count() - 1;
                    if (splittedMessage[0] == "path")
                    {
                        if (connections.Count - 1 >= 0)
                        {
                            if (connections.Last().startSlot == Convert.ToInt32(splittedMessage[lastIndex - 1]) && connections.Last().endSlot == Convert.ToInt32(splittedMessage[lastIndex]))
                            {
                                for (int i = 3; i < lastIndex - 1; i++)
                                {
                                    connections.Last().SNPPAddresses.Add(splittedMessage[i]);
                                }
                            }
                            else
                            {
                                connections.Add(new Connection(Convert.ToInt32(splittedMessage[lastIndex - 1]), Convert.ToInt32(splittedMessage[lastIndex])));
                                for (int i = 1; i < lastIndex - 1; i++)
                                {
                                    connections.Last().SNPPAddresses.Add(splittedMessage[i]);
                                }
                            }
                        }
                        else
                        {
                            connections.Add(new Connection(Convert.ToInt32(splittedMessage[lastIndex - 1]), Convert.ToInt32(splittedMessage[lastIndex])));
                            for (int i = 1; i < lastIndex - 1; i++)
                            {
                                connections.Last().SNPPAddresses.Add(splittedMessage[i]);
                            }
                        }
                        string st = string.Empty;
                        foreach (string s in connections.Last().SNPPAddresses)
                        {
                            st = st + " " + s;
                        }
                        //ReturnLog(st);
                    }
                    else if (splittedMessage[0] == "depath")
                    {
                        for (int i = 0; i < connections.Count; i++)
                        {
                            bool condition1 = splittedMessage[1] == connections[i].SNPPAddresses[0] && splittedMessage[2] == connections[i].SNPPAddresses.Last() && Convert.ToInt32(splittedMessage[3]) == connections[i].startSlot && Convert.ToInt32(splittedMessage[4]) == connections[i].endSlot;
                            bool condition2 = splittedMessage[2] == connections[i].SNPPAddresses[0] && splittedMessage[1] == connections[i].SNPPAddresses.Last() && Convert.ToInt32(splittedMessage[3]) == connections[i].startSlot && Convert.ToInt32(splittedMessage[4]) == connections[i].endSlot;
                            if (condition1 || condition2)
                            {
                                connections.RemoveAt(i);
                            }
                        }
                    }
                    else if (splittedMessage[0] == "repath")
                    {
                        int firstSwapIndex = 0, lastSwapIndex = 0;
                        foreach (Connection conn in connections)
                        {
                            if(splittedMessage[lastIndex - 1 ] == conn.startSlot.ToString() && splittedMessage[lastIndex] == conn.endSlot.ToString())
                            {
                                for(int i=0;i<conn.SNPPAddresses.Count; i++)
                                {
                                    if(conn.SNPPAddresses[i] == splittedMessage[1]) { firstSwapIndex = i; }
                                    if(conn.SNPPAddresses[i] == splittedMessage[lastIndex - 2]) { lastSwapIndex = i; }
                                }
                                for(int i = lastSwapIndex; i>= firstSwapIndex; i--)
                                {
                                    conn.SNPPAddresses.RemoveAt(i);
                                }
                                for (int i = 1; i <= lastIndex-2; i++)
                                {
                                    
                                    conn.SNPPAddresses.Insert(firstSwapIndex,splittedMessage[i]);
                                    firstSwapIndex++;
                                }
                            }
                        }
                    }
                    else if (splittedMessage[0] == "Message")
                    {
                        string sender;
                        string receiver;
                        Console.Write($"[{DateTime.Now}]" + " ");
                        if (splittedMessage[1] == "Natalka") { sender = Natalka; Console.Write("Message from Natalka "); }
                        else if (splittedMessage[1] == "Piotrek") { sender = Piotrek; Console.Write("Message from Piotrek "); }
                        else { sender = Tomek; Console.Write("Message from Tomek "); }

                        if (splittedMessage[2] == "Natalka") { receiver = Natalka; Console.Write("to Natalka. "); }
                        else if (splittedMessage[2] == "Piotrek") { receiver = Piotrek; Console.Write("to Piotrek. "); }
                        else { receiver = Tomek; Console.Write("to Tomek. "); }

                        string msg = splittedMessage[3];

                        if (splittedMessage.Count() > 4)
                        {
                            for (int i = 4; i < splittedMessage.Count(); i++)
                            {
                                msg += " " + splittedMessage[i];
                            }
                        }

                        foreach(Connection conn in connections)
                        {
                            if (conn.SNPPAddresses[0] == sender && conn.SNPPAddresses.Last() == receiver)
                            {
                                Console.Write("[From " + conn.startSlot + " to " + conn.endSlot + "]\n" + gap + "Connections:\n");
                                for (int i = 0; i<conn.SNPPAddresses.Count() - 1; i++)
                                {
                                    Console.WriteLine(gap + conn.SNPPAddresses[i] + " " + conn.SNPPAddresses[i + 1]);
                                }
                            }
                            else if (conn.SNPPAddresses[0] == receiver && conn.SNPPAddresses.Last() == sender)
                            {
                                Console.Write("[From " + conn.startSlot + " to " + conn.endSlot + "]\n" + gap + "Connections:\n");
                                conn.SNPPAddresses.Reverse();
                                for (int i = 0; i < conn.SNPPAddresses.Count() - 1; i++)
                                {
                                    Console.WriteLine(gap + conn.SNPPAddresses[i] + " " + conn.SNPPAddresses[i + 1]);
                                }
                                conn.SNPPAddresses.Reverse();
                            }
                            Console.WriteLine("---------------------");
                        }
                        if (receiver == Tomek) { SendMSG(TPort, "Message " + splittedMessage[1] + " " + msg);}
                        else if (receiver == Piotrek) { SendMSG(PPort, "Message " + splittedMessage[1] + " " + msg);}
                        else if (receiver == Natalka) { SendMSG(NPort, "Message " + splittedMessage[1] + " " + msg);}
                    }
                    else
                    {
                        ReturnLog("Wrong command.");
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                udpClient.Close();
            }
        }

        private static void ReturnLog(string log)
        {
            Console.WriteLine($"[{DateTime.Now}]" + " " + log + "\n---------------------");
        }

        public void ReadConfig(string path)
        {
            XmlReader reader = new XmlTextReader(path);

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name.ToString())
                    {
                        case "SNPP":
                            SNPPList.Add(new SNPP(reader.ReadString()));
                            break;
                        case "D1ADDRESS":
                            D1address = reader.ReadString();
                            break;
                        case "D2ADDRESS":
                            D2address = reader.ReadString();
                            break;
                        case "TOMEK":
                            Tomek = reader.ReadString();
                            break;
                        case "PIOTREK":
                            Piotrek = reader.ReadString();
                            break;
                        case "NATALKA":
                            Natalka = reader.ReadString();
                            break;
                        default:
                            break;
                    }
                }
            }
            ReturnLog("Configuration file loaded successfully");
        }

        public int ReturnIndex(string address)
        {
            int i = 0;
            foreach (SNPP snpp in SNPPList)
            {
                if(snpp.address == address)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        static void Main(string[] args)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            Program p = new Program();
            try
            {
                p.ReadConfig(arguments[1]);
            }
            catch
            {
                p.ReadConfig("CableCloudConfig.xml");
            }
            p.Start();
        }
    }
}
