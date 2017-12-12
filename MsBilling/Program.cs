using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Hosting.Self;

namespace MsBilling
{
    class Program
    {
        static void Main(string[] args)
        {
            const string serviceName = "Billing";
            string urlPortParam = args.Length == 0 ? "8001" : args[0];

            //var url = $"http://10.25.232.179:{urlPortParam}";

            var url = $"http://localhost:8001";

            var config = new HostConfiguration
            {
                RewriteLocalhost = true,
                UrlReservations = new UrlReservations { CreateAutomatically = true }
            };

            var host = new NancyHost(new Uri(url), new CustomBootstrapper(), config);
            host.Start();

            Console.WriteLine("{0} Running on {1}", serviceName, url);
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();

            host.Stop();
        }
    }
}
