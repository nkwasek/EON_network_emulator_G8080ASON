using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace KolejneCPCC
{
    class CPCC
    {
        private static string gap = "                            ";
        private string name;
        private string capReq;
        private string namReq;
        private int port;
        private int cpPort;
        private int CableCloudPort;
        private bool busy = false;

        UdpClient sender;
        UdpClient listener;

        Thread commandThread;
        Thread receiveThread;
        IPEndPoint RemoteIpEndPoint;
        private List<string> names = new List<string>();//lista prowadzonych polaczen

        public void Start()
        {
            ReturnLog($"Starting CPCC {name}...");
            commandThread = new Thread(MakeRequest);
            receiveThread = new Thread(ReceiveMessage);

            try
            {
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
            for (int i=0;i<names.Count;i++)
            {
                msg = "CallTeardown " + name + " " + names[i];
                SendRequest(msg);
            }
            listener.Close();
            sender.Close();
            Environment.Exit(-1);
        }
        public bool Included(string a)
        {
            for(int i=0;i<names.Count;i++)
            {
                if (names[i] == a)
                    return true;
            }
            return false;
        }
        public void MakeRequest()
        {
            sender = new UdpClient(port + 1);

            while (true)
            {
                string rl = Console.ReadLine();
                string[] tmp = rl.Split(' ');
                
                int len = tmp.Length;
                string msg = String.Empty;

                if (len == 3 && tmp[0] == "call" && busy == false && tmp[1]!=name)
                {
                    msg = "CallRequest " + name + " " + tmp[1] + " " + tmp[2]+" REQUEST";
                    SendRequest(msg);
                    ReturnLog("[CPCC] -> NCC : " + msg);
                    //busy = true;
                }
                else if (len >= 3 && tmp[0] == "msg")
                {
                    string receiver = tmp[1];
                    if (tmp[0] == "msg" && Included(receiver) && busy == false)
                    {
                        msg = "Message " + name + " " + receiver;

                        for (int i = 2; i < tmp.Length; i++)
                        {
                            msg += " " + tmp[i];
                        }

                        SendMsg(CableCloudPort, msg);
                    }
                    else
                    {
                        Console.WriteLine("Unable to send message.");
                    }
                   
                }
                else if (tmp[0] == "stop" && busy == false)
                {
                    Stop();
                }
                else if ((tmp[0] == "accept" || tmp[0]=="A") && busy == true)
                {
                    busy = false;
                    msg = "CallAccept "+ namReq +" "+ name +" "+capReq+" ACCEPTED";
                    SendRequest(msg);
                    names.Add(namReq);
                    ReturnLog("[CPCC] -> NCC : " + msg);
                    ReturnLog("Connection accepted.");
                }
                else if((tmp[0] == "reject" || tmp[0] == "R")  && busy == true)
                {
                    busy = false;
                    msg = "CallAccept " + namReq + " " + name + " " + capReq + " REJECTED";
                    SendRequest(msg);
                    ReturnLog("[CPCC] -> NCC : " + msg);
                    ReturnLog("Connection rejected.");

                }
                else if (len == 2 && busy == false)
                {
                    if (tmp[0] == "teardown")
                    {
                        msg = "CallTeardown " + name + " " + tmp[1];
                        Byte[] b = Encoding.ASCII.GetBytes(msg);
                        Send(sender, b);
                        ReturnLog("[CPCC] -> NCC : " + msg);
                        ReturnLog("Connection terminated.");
                    }
                    else
                        ReturnLog("Unknown command");
                }
                else
                {
                    ReturnLog("Unknown command");
                }
            }
        }
        
        public void SendRequest(string msg)
        {
            sender.Connect("localhost", cpPort);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
            Send(sender, sendBytes);                    
        }
        public void ReceiveMessage()
        {
            while (true)
            {
                listener = new UdpClient(port);
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);

                Byte[] receiveBytes = listener.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                string[] returnData2 = returnData.Split(' ');

                if (returnData2[0] == "CallAccept" && returnData2[4] == "REQUEST")
                {

                    busy = true;
                    ReturnLog("[CPCC] RECEIVED MESSAGE : " + returnData + "\n " + gap + "FROM: NCC ");
                    ReturnLog("Trying to connect from: " + returnData2[1] +"; Requested Capacity: " + returnData2[3]);
                    namReq = returnData2[1];
                    capReq = returnData2[3];
                    Console.WriteLine("accept[A]/reject[R] ?");
                }
                else
                if(returnData2[0] == "CallRequest")
                {
                    if (returnData2[4] == "REJECTED")
                    {
                        ReturnLog("[CPCC] RECEIVED MESSAGE : " + returnData + "\n " + gap + "FROM: NCC ");
                        busy = false;
                        ReturnLog("Can't be connected with: " + returnData2[2]);
                    }
                    else
                    if (returnData2[4] == "ACCEPTED")
                    {
                        ReturnLog("[CPCC] RECEIVED MESSAGE : " + returnData + "\n " + gap + "FROM: NCC ");
                        busy = false;
                        ReturnLog("Connected with: " + returnData2[2]);
                        names.Add(returnData2[2]);
                    }
                }
                else
                if(returnData2[0] == "CallTeardown")
                {
                    ReturnLog("[CPCC] RECEIVED MESSAGE : " + returnData + "\n " + gap + "FROM: NCC ");
                    ReturnLog("Connection closed with: " + returnData2[1]);
                }
                else
                if(returnData2[0]== "Message")
                {
                    string text = string.Empty;
                    for(int i=2;i<returnData2.Length;i++)
                    {
                        text += returnData2[i] + " ";
                    }
                    ReturnLog("From: " + returnData2[1] + " Message: " + text);
                }
                listener.Close();
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
                        case "NAME":
                            name = reader.ReadString();
                            break;
                        case "PORT":
                            port = int.Parse(reader.ReadString());
                            break;
                        case "CPPORT":
                            cpPort = int.Parse(reader.ReadString());
                            break;
                        case "CCPORT":
                            CableCloudPort = int.Parse(reader.ReadString());
                            break;
                    }
                }
            }
            ReturnLog("Configuration file loaded successfully");
        }

        private static void Send(UdpClient udpClient, byte[] byteData)
        {
            udpClient.Send(byteData, byteData.Length);
        }

        public void SendMsg(int sendPort, string msg)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
            sender.Connect("localhost", sendPort);
            sender.Send(sendBytes, sendBytes.Length);
        }
    }
}

