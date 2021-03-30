using System;
using System.Collections.Generic;
using System.IO;

namespace ControlPlaneServer
{
    class UsersData
    {
        public string Username { get; set; }
        public int Port { get; set; }
        public int CapacityLimit { get; set; }
        public string Address;
        public int DomainID { get; set; }

        public UsersData(string name, int port, int maxCapacity, int ID, string address)
        {
            Username = name;
            Port = port;
            CapacityLimit = maxCapacity;
            DomainID = ID;
            Address = address;
        }
    }

    class UsersDataTable
    {
        public static List<UsersData> usersDataTable = new List<UsersData>();
        private static string path = "UsersDataConfig.xml";

        public static void AddUserData(string name, int port, int capacity, int ID, string address)
        {
            usersDataTable.Add(new UsersData(name, port, capacity, ID, address));
        }

        public static int[] ReturnPortAndDomain(string name)
        {
            int[] tab = new int[2];
            foreach (UsersData row in usersDataTable)
            {
                if (row.Username == name)
                {
                    tab[0] = row.Port;
                    tab[1] = row.DomainID;
                    return tab;
                }
            }
            return tab;
        }
        public static string ReturnAddress(string name)
        {
            foreach (UsersData row in usersDataTable)
            {
                if (row.Username == name)
                {
                    return row.Address;
                }
            }
            return "null";
        }

        public static Boolean CheckCapacityLimit(string name, int val)
        {
            foreach (UsersData row in usersDataTable)
            {
                if (row.Username == name && row.CapacityLimit >= val)
                {
                    return true;
                }
            }
            return false;
        }

        public static int ReturnCapacityLimit(string name)
        {
            foreach (UsersData row in usersDataTable)
            {
                if (row.Username == name)
                {
                    return row.CapacityLimit;
                }
            }
            return 0;
        }

        public static void ReadConfig()     // logi tutaj można będzie usunąć
        {
            try
            {
                string[] config = File.ReadAllLines(path);
                foreach (string line in config)
                {
                    string[] splittedLine = line.Split(' ');
                    AddUserData(splittedLine[0], Convert.ToInt32(splittedLine[1]), Convert.ToInt32(splittedLine[2]), Convert.ToInt32(splittedLine[3]), splittedLine[4]);
                }
                ReturnLog("Configuration file loaded successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "\n");
                ReturnLog("Cannot load the configuration file");
            }
        }

        public static string returnName(int nr)
        {
            foreach (UsersData row in usersDataTable)
            {
                if (row.Port == nr)
                {
                    return row.Username;
                }
            }
            return null;
        }

        private static void ReturnLog(string log)
        {
            Console.WriteLine($"[{DateTime.Now}]" + " " + log + "\n---------------------");
        }
    }
}
