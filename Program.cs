using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check if the application is running as a server or a client
            if (args.Length > 0 && args[0] == "server")
            {
                // Run the server
                Server.Start();
            }
            else
            {
                // Run the client
                Client.Start();
            }
        }
    }
}