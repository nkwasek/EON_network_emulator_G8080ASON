using System;

namespace ControlPlaneServer
{
    class Policy
    {
        private static string gap = "\n                          ";
        private static string line = "------------------------------------";
        public static Boolean CAC(string username1, string username2, int capacity) // Call Admission Control
        {
            Boolean checkCallingPCC = UsersDataTable.CheckCapacityLimit(username1, capacity);
            Boolean checkCalledPCC = UsersDataTable.CheckCapacityLimit(username2, capacity);
            int cap1 = UsersDataTable.ReturnCapacityLimit(username1);
            int cap2 = UsersDataTable.ReturnCapacityLimit(username2);

            string msg = "Call Admission Control : " +gap + line + gap + "Demanded capacity: " + capacity + gap + "Capacity limit for user " + username1 + ": " + cap1 + gap + "Capacity limit for user " + username2 + ": " + cap2;
            msg += gap + line;
            msg += gap + cap1;

            if(cap1 < capacity)
            {
                msg += " < ";
            }
            else if (cap1 > capacity)
            {
                msg += " > ";
            }
            else if (cap1 == capacity)
            {
                msg += " = ";
            }

            msg += capacity + "  &&  " + cap2;

            if (cap2 < capacity)
            {
                msg += " < ";
            }
            else if (cap2 > capacity)
            {
                msg += " > ";
            }
            else if (cap2 == capacity)
            {
                msg += " = ";
            }

            msg += capacity;

            msg += gap + line;

            if (checkCalledPCC && checkCallingPCC)
            {
                msg += gap + "Admission confirmed";
            }
            else
            {
                msg += "\n" + gap + "Admission denied";
            }

            ReturnLog(msg);

            if (checkCalledPCC && checkCallingPCC)
            {
                ReturnLog("-> NCC : Policy Response: Admission confirmed.");
                return true;
            }
            else
            {
                ReturnLog("-> NCC : Policy Response: Admission denied.");
                return false;
            }
        }
        private static void ReturnLog(string log)
        {
            Console.WriteLine($"[{DateTime.Now}]" + " [P] " + log + "\n---------------------");
        }
    }
}
