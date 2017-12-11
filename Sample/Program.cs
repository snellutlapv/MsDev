using Nancy.Hosting.Self;
using System;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
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

            Console.WriteLine("Running on {0}", url);
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();

            host.Stop();
        }
    }
}
