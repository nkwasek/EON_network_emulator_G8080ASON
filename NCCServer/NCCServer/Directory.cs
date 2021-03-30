using System;

namespace ControlPlaneServer
{
    class Directory
    { 
        public static int[] DirectoryRequest(string username)
        {
            int[] tab = new int[2];

            tab[0] = UsersDataTable.ReturnPortAndDomain(username)[0]; //port
            tab[1] = UsersDataTable.ReturnPortAndDomain(username)[1]; // ID domeny
            string Address = UsersDataTable.ReturnAddress(username);

            ReturnLog("-> NCC : DirectoryRequest Response: Username \"" + username + "\" translated into address " + Address + ":" + tab[0]);
            return tab;
        }

        private static void ReturnLog(string log)
        {
            Console.WriteLine($"[{DateTime.Now}]" + " [D] " + log + "\n---------------------");
        }
    }
}
