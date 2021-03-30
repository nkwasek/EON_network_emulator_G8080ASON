using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace ManagementCenter
{
    class Program
    {
        private static string gap = "                            ";
        private int MCPort;
        private int NCCPort;
        private int CCPort;
        UdpClient udpClient;
        Thread commandThread;
        Thread receiveThread;
        IPEndPoint RemoteIpEndPoint;

        public void Start()
        {
            ReturnLog("Starting Management Center...");
            commandThread = new Thread(MakeRequest);
            receiveThread = new Thread(ReceiveMessage);

            try
            {
                udpClient = new UdpClient(MCPort);
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

                if (tmp[0] == "CallRequest")
                {
                    msg = "CallRequest " + tmp[1] + " " + tmp[2] + " " + tmp[3] + " REQUEST";
                    SendRequest(NCCPort, msg);
                    ReturnLog("[Management Center] -> NCC " + " : " + msg);
                }
                else
                //
                if (tmp[0] == "break")
                {
                    msg = "break " + tmp[1];
                    SendRequest(CCPort, msg);
                    ReturnLog("[Management Center] -> NCC " + " : " + msg);
                }
                else
                if (tmp[0] == "repair")
                {
                    msg = "repair " + tmp[1];
                    SendRequest(CCPort, msg);
                    ReturnLog("[Management Center] -> NCC " + " : " + msg);
                }
                //
                else
                if (tmp[0] == "NodeConnections")
                {
                    msg = "NodeConnections " + tmp[1];
                    SendRequest(CCPort, msg);
                    ReturnLog("[Management Center] -> NCC " + " : " + msg);
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

        public void ReceiveMessage()
        {
            while (true)
            {
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                string[] returnData2 = returnData.Split(' ');
                ReturnLog("Received : " + returnData);
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
                        case "MCPORT":
                            MCPort = int.Parse(reader.ReadString());
                            break;
                        case "NCCPORT":
                            NCCPort = int.Parse(reader.ReadString());
                            break;
                        case "CCPORT":
                            CCPort = int.Parse(reader.ReadString());
                            break;
                    }
                }
            }
            ReturnLog("Configuration file loaded successfully");
        }

        static void Main(string[] args)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            Program p = new Program();
            p.ReadConfig(arguments[1]);
            p.Start();
        }
    }
}
