using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(EcgService)))
            {
                try
                {
                    host.Open();
                    Console.WriteLine("========================================");
                    Console.WriteLine("              SERVER JE POKRENUT");
                    Console.WriteLine("   Adresa: net.tcp://localhost:4000/EcgService");
                    Console.WriteLine("========================================");
                    Console.WriteLine("Pritisni [Enter] da ugasiš server...");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Greška pri pokretanju: {ex.Message}");
                    Console.ReadLine();
                }
            }
        }
    }
}
