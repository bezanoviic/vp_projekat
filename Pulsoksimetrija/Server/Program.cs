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
                    Console.WriteLine("             SERVER IS RUNNING");
                    Console.WriteLine("   Address: net.tcp://localhost:4000/EcgService");
                    Console.WriteLine("========================================");
                    Console.WriteLine("Press [Enter] to shut down the server...");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error starting server: {ex.Message}");
                    Console.ReadLine();
                }
            }
        }
    }
}
