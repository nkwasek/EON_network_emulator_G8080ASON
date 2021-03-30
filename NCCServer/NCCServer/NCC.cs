using System;

namespace ControlPlaneServer
{
    class NCC
    {
        public static int CallingPCCPort;
        public static int CalledPCCPort;
        public static string CallingName;
        public static string CalledName;
        public static string TCPCCNames;
        public static string CPCCNames;
        public static int TCallingPCCPort;
        public static int TCalledPCCPort;
        public static int CallingPCCDomain;
        public static int CalledPCCDomain;
        public static int TCallingPCCDomain;
        public static int TCalledPCCDomain;
        public static int DemandedCapacity;
        public static string CallingAddress;
        public static string CalledAddress;


        public static Boolean CallRequest(string CallingPCCName, string CalledPCCName, int demandedCapacity)
        {

            CPCCNames = CallingPCCName + " " + CalledPCCName;
            DemandedCapacity = demandedCapacity;

            CallingName = CallingPCCName;
            CalledName = CalledPCCName;

            CalledPCCPort = UsersDataTable.ReturnPortAndDomain(CalledPCCName)[0];       // port
            CalledPCCDomain = UsersDataTable.ReturnPortAndDomain(CalledPCCName)[1];     // ID domeny


            CallingPCCPort = UsersDataTable.ReturnPortAndDomain(CallingPCCName)[0];     // port
            CallingPCCDomain = UsersDataTable.ReturnPortAndDomain(CallingPCCName)[1];   // ID domeny

            CallingAddress = UsersDataTable.ReturnAddress(CallingPCCName) + ":" + CallingPCCPort;
            CalledAddress = UsersDataTable.ReturnAddress(CalledPCCName) + ":" + CalledPCCPort;


            ReturnLog(" -> P Policy " + CallingPCCName + " " + CalledPCCName + " " + demandedCapacity);

            if (Policy.CAC(CallingPCCName, CalledPCCName, demandedCapacity))
            {
                ReturnLog("-> D : DirectoryRequest " + CallingPCCName);
                Directory.DirectoryRequest(CallingPCCName);

                ReturnLog("-> D : DirectoryRequest " + CalledPCCName);
                Directory.DirectoryRequest(CalledPCCName);

                return true;
            }
            else { return false; }
            
        }

        public static Boolean CallCoordination(string CallingPCCName, string CalledPCCName, int demandedCapacity)
        { 
            CPCCNames = CallingPCCName + " " + CalledPCCName;
            CallingName = CallingPCCName;
            CalledName = CalledPCCName;

            CalledPCCPort = UsersDataTable.ReturnPortAndDomain(CalledPCCName)[0];       // port
            CalledPCCDomain = UsersDataTable.ReturnPortAndDomain(CalledPCCName)[1];     // ID domeny

            DemandedCapacity = demandedCapacity;

            CallingPCCPort = UsersDataTable.ReturnPortAndDomain(CallingPCCName)[0];     // port
            CallingPCCDomain = UsersDataTable.ReturnPortAndDomain(CallingPCCName)[1];   // ID domeny

            CallingAddress = UsersDataTable.ReturnAddress(CallingPCCName) + ":" + CallingPCCPort;
            CalledAddress = UsersDataTable.ReturnAddress(CalledPCCName) + ":" + CalledPCCPort;

            ReturnLog("-> P : Policy " + CallingPCCName + " " + CalledPCCName + " " + demandedCapacity);

            if (Policy.CAC(CallingPCCName, CalledPCCName, demandedCapacity))
            {
                return true;
            }
            else { return false; }
        }

        public static void CallTeardown(string callingPCCName, string calledPCCName)
        {
            int[] tab = new int[2];

            CPCCNames = callingPCCName + " " + calledPCCName;

            CallingName = callingPCCName;
            CalledName = calledPCCName;

            CalledPCCPort = UsersDataTable.ReturnPortAndDomain(calledPCCName)[0];       //port
            CalledPCCDomain = UsersDataTable.ReturnPortAndDomain(calledPCCName)[1];     // ID domeny

            CallingPCCPort = UsersDataTable.ReturnPortAndDomain(callingPCCName)[0];     //port
            CallingPCCDomain = UsersDataTable.ReturnPortAndDomain(callingPCCName)[1];   // ID domeny

            CallingAddress = UsersDataTable.ReturnAddress(callingPCCName) + ":" + CallingPCCPort;
            CalledAddress = UsersDataTable.ReturnAddress(calledPCCName) + ":" + CalledPCCPort;
        }

        public static void PassMessage(string name1, string name2)
        {
            CalledPCCPort = UsersDataTable.ReturnPortAndDomain(name2)[0];       //port
            CalledPCCDomain = UsersDataTable.ReturnPortAndDomain(name1)[1];     // ID domeny

            CallingPCCPort = UsersDataTable.ReturnPortAndDomain(name2)[0];     //port
            CallingPCCDomain = UsersDataTable.ReturnPortAndDomain(name2)[1];   // ID domeny
        }

        public static void Unable(string addr1, string addr2)
        {
            string port1 = addr1.Split(':')[1];
            string port2 = addr2.Split(':')[1];

            string name1 = UsersDataTable.returnName(Convert.ToInt32(port1));
            string name2 = UsersDataTable.returnName(Convert.ToInt32(port2));

            CallingName = name1;
            CalledName = name2;

            CPCCNames = name1 + " " + name2;

            CallingPCCPort = UsersDataTable.ReturnPortAndDomain(name1)[0];       // port
            CallingPCCDomain = UsersDataTable.ReturnPortAndDomain(name1)[1];     // ID domeny

            CalledPCCPort = UsersDataTable.ReturnPortAndDomain(name2)[0];     // port
            CalledPCCDomain = UsersDataTable.ReturnPortAndDomain(name2)[1];   // ID domeny
        }

        public static void SwapAddresses()
        {
            int tmpPort = CallingPCCPort;
            string tmpAddress = CallingAddress;
            int tmpDomain = CallingPCCDomain;

            CallingPCCPort = CalledPCCPort;
            CallingAddress = CalledAddress;
            CallingPCCDomain = CalledPCCDomain;

            CalledPCCPort = tmpPort;
            CalledAddress = tmpAddress;
            CalledPCCDomain = tmpDomain;

        }

        public static void ReturnLog(string log)
        {
            Console.WriteLine($"[{DateTime.Now}]" + " [NCC] " + log + "\n---------------------");
        }
    }
}
