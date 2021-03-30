using System;
using System.Collections.Generic;
using System.Text;

namespace KolejneCPCC
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            CPCC cpcc = new CPCC();
            cpcc.ReadConfig(arguments[1]);
            cpcc.Start();
        }
    }
}
