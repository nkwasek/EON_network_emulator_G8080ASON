using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ControlPlaneServer
{
    class CPServer
    {
        private static int listenPort;
        private static int nextDomainPort;
        private static int CCPort;
        private static int DomainID;
        private static string ExternalPort;
        private static string currentNCCReq;
        private static int port;
        private static string startPort;
        private static string DomainAddress = "127.0.0.1";
        private static UdpClient listener;
        private static UdpClient sender;
        private static string gap = "                           ";
        private static string slotsNumber;
        private static string slotLastIndex;

        public static string CoordinationCONFIRMED = "CallCoordination Response " + NCC.CPCCNames + " " + NCC.CallingPCCPort + " " + startPort + " " + NCC.CalledPCCPort + " " + NCC.DemandedCapacity + " CONFIRMED";
        public static string CoordinationREJECTED = "CallCoordination Response " + NCC.CPCCNames + " " + NCC.CallingPCCPort + " "+ startPort + " " + NCC.CalledPCCPort + " " + NCC.DemandedCapacity + " REJECTED";
        public static string CoordinationTEARDOWN = "CallCoordination " + NCC.CPCCNames + " " + NCC.CallingAddress + " " + startPort + " " + NCC.CalledAddress + " " + NCC.DemandedCapacity + " TEARDOWN";
        public static string CoordinationREQ = "CallCoordination Request " + NCC.CPCCNames + " " + NCC.CallingPCCPort + " " + startPort + " " + NCC.CalledPCCPort + " " + NCC.DemandedCapacity + " " + slotsNumber + " " + slotLastIndex;
        public static string CallReqACCEPTED = "CallRequest " + NCC.CPCCNames + " " + NCC.DemandedCapacity + " ACCEPTED";
        public static string CallReqREJECTED = "CallRequest " + NCC.CPCCNames + " " + NCC.DemandedCapacity + " REJECTED";
        public static string ConnReqSETUP = "ConnectionRequest " + NCC.CallingPCCPort + " " + startPort + " " + NCC.CalledPCCPort + " " + NCC.DemandedCapacity + " SETUP";
        public static string ConnReqRELEASE = "ConnectionRequest " + NCC.CallingPCCPort + " " + NCC.CalledPCCPort + " RELEASE";
        public static string ConnReqCoordSETUP = "ConnectionRequest " + NCC.CallingPCCPort + " " + startPort + " " + NCC.CalledPCCPort + " " + NCC.DemandedCapacity + " " + slotsNumber + " " + slotLastIndex + " SETUP";
        public static string CallAcceptREQ = "CallAccept " + NCC.CPCCNames + " " + NCC.DemandedCapacity + " REQUEST";
        public static string CallTeardown = "CallTeardown " + NCC.CPCCNames;

        public static void StartListener()
        {
            try
            {
                while (true)
                {
                    listener = new UdpClient(listenPort);
                    sender = new UdpClient(listenPort + 1);
                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
                    
                    byte[] bytes = listener.Receive(ref groupEP);
                    string message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    
                    string[] splittedMessage = message.Split(' ');

                    if (splittedMessage[0] == "CallRequest") // Przykład CallRequesta: "CallRequest Natalka Piotr [capacity] REQUEST"
                    {

                        currentNCCReq = "CallRequest";
                        if (splittedMessage.Last() == "REQUEST")
                        {
                            NCC.ReturnLog("RECEIVED MESSAGE: " + message + "\n " + gap + "FROM: CPCC " + splittedMessage[1]);
                            Boolean CAC = NCC.CallRequest(splittedMessage[1], splittedMessage[2], Convert.ToInt32(splittedMessage[3]));
                            startPort = NCC.CallingAddress;
                            updateLogs();

                            if (CAC) // jesli autoryzacja poszla spoko
                            {
                                send(CCPort, ConnReqSETUP);
                                NCC.ReturnLog("-> CC : " + ConnReqSETUP);
                            }
                            else // żądane połączenie nie przeszło autoryzacja
                            {
                                send(NCC.CallingPCCPort, CallReqREJECTED);
                                NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CallReqREJECTED);
                            }
                        }
                        else if (splittedMessage.Last() == "ACCEPTED")
                        {
                            updateLogs();
                            NCC.ReturnLog("RECEIVED MESSAGE: " + message + "\n " + gap + "FROM: CPCC " + splittedMessage[1]);

                            if (DomainID != NCC.CallingPCCDomain)     // w przypadku wielu domen przekazujemy response na CallRequesta między NCC
                            {
                                send(nextDomainPort, CoordinationCONFIRMED);
                                NCC.ReturnLog("-> NCC : " + CoordinationCONFIRMED);
                            }
                            else                                    // przesyłamy CallRequest Response do CPCC
                            {
                                send(NCC.CallingPCCPort, CallReqACCEPTED);
                                NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CallReqACCEPTED);
                            }
                        }
                        else if (splittedMessage.Last() == "REJECTED")
                        {
                            NCC.ReturnLog("RECEIVED MESSAGE: " + message + "\n " + gap + "FROM: CPCC " + splittedMessage[1]);
                            send(CCPort, ConnReqRELEASE);
                            NCC.ReturnLog("-> CC : " + ConnReqRELEASE);
                        }
                        else if (splittedMessage.Last() == "MANAGEMENTREQUEST")
                        {
                            NCC.ReturnLog("RECEIVED MESSAGE: " + message + "\n " + gap + "FROM: Management Center");

                            Boolean CAC = NCC.CallRequest(splittedMessage[1], splittedMessage[2], Convert.ToInt32(splittedMessage[3]));
                            startPort = NCC.CallingAddress;
                            updateLogs();

                            if (CAC) // jesli autoryzacja poszla spoko
                            {
                                if (NCC.CallingPCCDomain != DomainID)
                                {
                                    NCC.SwapAddresses();
                                    startPort = NCC.CallingAddress;
                                    updateLogs();
                                }
                                send(CCPort, ConnReqSETUP);
                                NCC.ReturnLog("-> CC : " + ConnReqSETUP);
                            }
                            else // żądane połączenie nie przeszło autoryzacja
                            {
                                send(NCC.CallingPCCPort, CallReqREJECTED);
                                NCC.ReturnLog("-> Management Center : " + CallReqREJECTED);
                            }
                        }
                    }
                    else if (splittedMessage[0] == "ConnectionRequest")
                    {
                        NCC.ReturnLog("RECEIVED MESSAGE: " + message + "\n " + gap + "FROM: CC");

                        if (splittedMessage.Last() == "SET")
                        {
                            slotsNumber = splittedMessage[5];
                            slotLastIndex = splittedMessage[6];

                            if (DomainID != NCC.CalledPCCDomain)     // w przypadku wielu domen i robimy dalej CallCoordination między NCC
                            {
                                startPort = splittedMessage[2];
                                updateLogs();
                                send(nextDomainPort, CoordinationREQ);
                                NCC.ReturnLog("-> NCC : " + CoordinationREQ);
                            }
                            else                                    // jak już mamy zestawione polaczenie to wysylamy CallAccept
                            {
                                send(NCC.CalledPCCPort, CallAcceptREQ); //  ACCEPTED / REJECTED
                                NCC.ReturnLog("-> CPCC " + NCC.CalledName + " : " + CallAcceptREQ);
                            }
                        }
                        else if (splittedMessage.Last() == "RELEASED")
                        {
                            if(currentNCCReq == "CallRequest")
                            {
                                if (DomainID != NCC.CallingPCCDomain)     // musimy przekazać CallTeardown dalej
                                {
                                    send(NCC.CallingPCCPort, CoordinationREJECTED);
                                    NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CoordinationREJECTED);
                                }
                                else                                      // jestesmy w domenie doecelowej
                                {
                                    send(NCC.CallingPCCPort, CallReqREJECTED);
                                    NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CallReqREJECTED);
                                }
                            }
                            else if (currentNCCReq == "CallTeardown")
                            {
                                if (DomainID != NCC.CalledPCCDomain)     // musimy przekazać CallTeardown dalej
                                {
                                    send(nextDomainPort, CoordinationTEARDOWN);
                                    NCC.ReturnLog("-> NCC : " + CoordinationTEARDOWN);
                                }
                                else                                      // jestesmy w domenie doecelowej
                                {
                                    string text = "CallTeardown " + NCC.CPCCNames;
                                    send(NCC.CalledPCCPort, text);
                                    NCC.ReturnLog("-> CPCC " + NCC.CalledName + " : " + text);
                                }
                            }
                            else if(currentNCCReq == "CallAccept")
                            {
                                if(DomainID != NCC.CallingPCCDomain)
                                {
                                    send(nextDomainPort, CoordinationREJECTED);
                                    NCC.ReturnLog("-> NCC : " + CoordinationREJECTED);
                                }
                                else
                                {
                                    send(NCC.CallingPCCPort, CallReqREJECTED);
                                    NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CallReqREJECTED);
                                }
                            }
                            else if(currentNCCReq.Split(' ')[0] == "CallCoordination") // BLAD
                            {
                                send(NCC.CallingPCCPort, CallReqREJECTED);
                                NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CallReqREJECTED);
                            }
                            else if (currentNCCReq == "CoordinationTeardown")
                            {
                                updateLogs();
                                send(NCC.CalledPCCPort, CallTeardown);
                                NCC.ReturnLog("-> CPCC " + NCC.CalledName + " : " + CallTeardown);

                            }
                            else if(currentNCCReq == "Released")
                            {
                                updateLogs();
                                Console.WriteLine("calling "+NCC.CallingName);
                                Console.WriteLine("called " + NCC.CalledName);

                                if (NCC.CallingPCCDomain == DomainID)
                                {
                                    //Console.WriteLine("calling");
                                    send2(NCC.CallingPCCPort, CallReqREJECTED);
                                    NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CallReqREJECTED);
                                }
                                else if (NCC.CalledPCCDomain == DomainID)
                                {
                                    //Console.WriteLine("called");
                                    string txt = "CallRequest " + NCC.CalledName + " " + NCC.CallingName + " " + NCC.DemandedCapacity + " REJECTED";
                                    send2(NCC.CalledPCCPort, txt);
                                    NCC.ReturnLog("-> CPCC " + " : " + txt);
                                }
                            }
                        }
                        else if (splittedMessage.Last() == "UNABLE")
                        {
                            if (currentNCCReq == "CallCoordination Request")
                            {
                                send(nextDomainPort, CoordinationREJECTED);
                                NCC.ReturnLog("-> NCC : " + CoordinationREJECTED);
                            }
                            else if (currentNCCReq == "CallRequest")
                            {
                                send(NCC.CallingPCCPort, CallReqREJECTED);
                                NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CallReqREJECTED);
                            }
                            else
                            {
                                NCC.Unable(splittedMessage[1], splittedMessage[3]);
                                startPort = splittedMessage[2];
                                NCC.DemandedCapacity = Convert.ToInt32(splittedMessage[4]);
                                string txt = "CallCoordination " + NCC.CPCCNames + " " + NCC.CallingAddress + " " + startPort + " " + NCC.CalledAddress + " " + NCC.DemandedCapacity + " RELEASE";
                                updateLogs();

                                if(NCC.CallingPCCDomain != NCC.CalledPCCDomain)
                                {
                                    send(nextDomainPort, txt);
                                    NCC.ReturnLog("-> NCC : " + txt);

                                    //Console.WriteLine(NCC.CalledPCCDomain + " " + NCC.CallingPCCDomain);

                                    if(NCC.CallingPCCDomain == DomainID)
                                    {
                                        //Console.WriteLine("calling");
                                        send2(NCC.CallingPCCPort, CallReqREJECTED);
                                        NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CallReqREJECTED);
                                    }
                                    else if (NCC.CalledPCCDomain == DomainID)
                                    {
                                        //Console.WriteLine("called");
                                        string txxt = "CallRequest " + NCC.CalledName + " " + NCC.CallingName + " " + NCC.DemandedCapacity + " REJECTED";
                                        send2(NCC.CalledPCCPort, txxt);
                                        NCC.ReturnLog("-> CPCC " + NCC.CalledName + " : " + txxt);
                                    }
                                }
                                else
                                {
                                    send(NCC.CallingPCCPort, CallReqREJECTED);
                                    NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CallReqREJECTED);
                                    send2(NCC.CalledPCCPort, CallReqREJECTED);
                                    NCC.ReturnLog("-> CPCC " + NCC.CalledName + " : " + CallReqREJECTED);
                                }
                            }
                        }
                    }
                    else if (splittedMessage[0] == "CallCoordination")
                    {
                        NCC.ReturnLog("RECEIVED MESSAGE: " + message + "\n " + gap + "FROM: NCC");

                        if (splittedMessage[1] == "Request")
                        {
                            currentNCCReq = "CallCoordination Request";
                            startPort = splittedMessage[5];
                            Boolean CAC = NCC.CallCoordination(splittedMessage[2], splittedMessage[3], Convert.ToInt32(splittedMessage[7]));

                            slotsNumber = splittedMessage[8];
                            slotLastIndex = splittedMessage[9];
                            
                            updateLogs();

                            if (CAC) // jesli autoryzacja poszla spoko
                            {
                                if (NCC.CallingPCCDomain == DomainID)
                                {
                                    NCC.SwapAddresses();
                                    updateLogs();
                                }

                                send(CCPort, ConnReqCoordSETUP);
                                NCC.ReturnLog("-> CC : " + ConnReqCoordSETUP);
                            }
                            else // nie przeszła autoryzacja
                            {
                                string text = "CallRequest " + splittedMessage[2] + " " + splittedMessage[3] + " " + splittedMessage[7] + " REJECTED";
                                send(NCC.CalledPCCPort, text); // REJECTED
                                NCC.ReturnLog("-> CPCC " + NCC.CalledName + " : " + text);
                            }
                        }
                        else if (splittedMessage[1] == "Response")
                        {
                            currentNCCReq = "CallCoordination Response";
                            if (splittedMessage.Last() == "CONFIRMED")
                            {
                                send(NCC.CallingPCCPort, CallReqACCEPTED);
                                NCC.ReturnLog("-> CPCC " + NCC.CallingName + " : " + CallReqACCEPTED);
                            }
                            else if (splittedMessage.Last() == "REJECTED")
                            {
                                send(CCPort, ConnReqRELEASE);
                                NCC.ReturnLog("-> CC : " + ConnReqRELEASE);
                            }
                        }
                        else if(splittedMessage.Last() == "RELEASE")
                        {
                            currentNCCReq = "Released";
                            NCC.CallTeardown(splittedMessage[1], splittedMessage[2]); //to nie teardown
                            send(CCPort, ConnReqRELEASE);
                            NCC.ReturnLog("-> CC : " + ConnReqRELEASE);
                        }
                        else if (splittedMessage.Last() == "TEARDOWN")
                        {
                            currentNCCReq = "CoordinationTeardown";
                            NCC.CallTeardown(splittedMessage[1], splittedMessage[2]); // tu zmieniane
                            updateLogs();
                            send(CCPort, ConnReqRELEASE);
                            NCC.ReturnLog("-> CC : " + ConnReqRELEASE);
                        }
                    }
                    else if (splittedMessage[0] == "CallAccept")
                    {
                        currentNCCReq = "CallAccept";

                        NCC.ReturnLog("RECEIVED MESSAGE: " + message + "\n " + gap + "FROM: CPCC " + splittedMessage[2]);

                        int port = Convert.ToInt32(groupEP.ToString().Split(':')[1]);

                        if(splittedMessage.Last() == "REJECTED")
                        {
                            send(CCPort, ConnReqRELEASE);
                            NCC.ReturnLog("-> CC : " + ConnReqRELEASE);
                        }
                        else
                        {
                            if (DomainID != NCC.CallingPCCDomain)     // musimy przekazać dalej
                            {
                                send(nextDomainPort, CoordinationCONFIRMED);
                                NCC.ReturnLog("-> NCC : " + CoordinationCONFIRMED);
                            }
                            else                                     // jestesmy w domenie doecelowej
                            {
                                send(NCC.CallingPCCPort, CallReqACCEPTED);
                                NCC.ReturnLog("-> CPCC "+ NCC.CallingName + " : " + CallReqACCEPTED);
                            }
                        }
                    }
                    else if (splittedMessage[0] == "CallTeardown")  // Przykład CallTeardown: "CallTeardown Natalka Piotrek"
                    {
                        currentNCCReq = "CallTeardown";

                        NCC.ReturnLog("RECEIVED MESSAGE: " + message + "\n " + gap + "FROM: CPCC " + splittedMessage[1]);

                        port = Convert.ToInt32(groupEP.ToString().Split(':')[1]);
                        NCC.CallTeardown(splittedMessage[1], splittedMessage[2]);
                        updateLogs();
                        send(CCPort, ConnReqRELEASE);
                        NCC.ReturnLog("-> CC : " + ConnReqRELEASE);
                    }

                    listener.Close();
                    sender.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
                sender.Close();
            }
        }

        private static void ReturnLog(string log)
        {
            Console.WriteLine($"[{DateTime.Now}]" + " " + log + "\n---------------------");
        }

        public static void ReadConfig(string path)
        {
            try
            {
                string[] config = File.ReadAllLines(path);
                foreach (string line in config)
                {
                    string[] splittedLine = line.Split(' ');
                    DomainID = Convert.ToInt32(splittedLine[0]);
                    listenPort = Convert.ToInt32(splittedLine[1]);
                    nextDomainPort = Convert.ToInt32(splittedLine[2]);
                    CCPort = Convert.ToInt32(splittedLine[3]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "\n");
            }
        }
        
        public static void send(int port, string text)
        {
            listener.Connect("localhost", port);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(text);
            listener.Send(sendBytes, sendBytes.Length);
        }

        public static void send2(int port, string text)
        {
            sender.Connect("localhost", port);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(text);
            sender.Send(sendBytes, sendBytes.Length);
        }

        public static void updateLogs()
        {
            CoordinationCONFIRMED = "CallCoordination Response " + NCC.CPCCNames + " " + NCC.CallingAddress + " " + startPort + " " + NCC.CalledAddress + " " + NCC.DemandedCapacity + " CONFIRMED";
            CoordinationREJECTED = "CallCoordination Response " + NCC.CPCCNames + " " + NCC.CallingAddress + " " + startPort + " " + NCC.CalledAddress + " " + NCC.DemandedCapacity + " REJECTED";
            CoordinationTEARDOWN = "CallCoordination " + NCC.CPCCNames + " " + NCC.CallingAddress + " " + startPort + " " + NCC.CalledAddress + " " + NCC.DemandedCapacity + " TEARDOWN";
            CoordinationREQ = "CallCoordination Request " + NCC.CPCCNames + " " + NCC.CallingAddress + " " + startPort + " " + NCC.CalledAddress + " " + NCC.DemandedCapacity + " " + slotsNumber + " " + slotLastIndex;
            CallReqACCEPTED = "CallRequest " + NCC.CPCCNames + " " + NCC.DemandedCapacity + " ACCEPTED";
            CallReqREJECTED = "CallRequest " + NCC.CPCCNames + " " + NCC.DemandedCapacity + " REJECTED";
            ConnReqSETUP = "ConnectionRequest " + NCC.CallingAddress + " " + startPort + " " + NCC.CalledAddress + " " + NCC.DemandedCapacity + " SETUP";
            ConnReqCoordSETUP = "ConnectionRequest " + NCC.CallingAddress + " " + startPort + " " + NCC.CalledAddress + " " + NCC.DemandedCapacity + " " + slotsNumber + " " + slotLastIndex + " SETUP";
            ConnReqRELEASE = "ConnectionRequest " + NCC.CallingAddress + " " + NCC.CalledAddress + " RELEASE";
            CallAcceptREQ = "CallAccept " + NCC.CPCCNames + " " + NCC.DemandedCapacity + " REQUEST";
            CallTeardown = "CallTeardown " + NCC.CPCCNames;
        }

        static void Main(string[] args)
        {
            Console.WriteLine();
            string[] arguments = Environment.GetCommandLineArgs();
            ReadConfig(arguments[1]);
            UsersDataTable.ReadConfig();
            StartListener();
            Console.ReadLine();
        }
    }
}
